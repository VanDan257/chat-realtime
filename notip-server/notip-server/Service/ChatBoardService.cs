using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using notip_server.Data;
using notip_server.Dto;
using notip_server.Hubs;
using notip_server.Models;
using notip_server.Interfaces;
using notip_server.Utils;
using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;
using notip_server.ViewModel.ChatBoard;
using System.Numerics;
using System.Net;
using System.Reflection;
using static notip_server.Utils.Constants;
using Newtonsoft.Json;
using notip_server.ViewModel.Friend;
using notip_server.ViewModel.User;
using System.Net.WebSockets;
using System.ComponentModel;
using Amazon.S3.Model;
using Amazon.S3;
using System.IO;
using notip_server.ViewModel.Common;

namespace notip_server.Service
{
    public class ChatBoardService : IChatBoardService
    {
        #region fields
        private readonly DbChatContext chatContext;
        private readonly IWebHostEnvironment webHostEnvironment;
        private ChatHub chatHub;
        private readonly IUserService _userService;
        private readonly IAwsS3Service _aswS3Service;

        #endregion

        #region ctor
        public ChatBoardService(DbChatContext chatContext, IWebHostEnvironment webHostEnvironment, ChatHub chatHub, IUserService userService, IAwsS3Service aswS3Service)
        {
            this.chatContext = chatContext;
            this.chatHub = chatHub;
            this.webHostEnvironment = webHostEnvironment;
            _userService = userService;
            _aswS3Service = aswS3Service;
        }

        #endregion

        /// <summary>
        /// Danh sách lịch sử chat
        /// </summary>
        /// <param name="userSession">User hiện tại đang đăng nhập</param>
        /// <returns>Danh sách lịch sử chat</returns>
        public async Task<List<GroupDto>> GetHistory(Guid userSession)
        {
            //Lấy danh sách nhóm chat
            List<GroupDto> groups = await chatContext.Groups
                    .Where(x => x.GroupUsers.Any(y => y.UserCode.Equals(userSession)))
                    .Select(x => new GroupDto()
                    {
                        Code = x.Code,
                        Name = x.Name,
                        Avatar = x.Avatar,
                        Type = x.Type,
                        LastActive = x.LastActive,
                        Users = x.GroupUsers.Select(y => new UserDto()
                        {
                            Id = y.User.Id,
                            UserName = y.User.UserName,
                            Avatar = y.User.Avatar,
                        }).ToList(),
                    }).ToListAsync();

            foreach (var group in groups)
            {
                //Nếu nhóm chat có type = SINGLE (chat 1-1) => đổi tên nhóm chat thành tên người chat cùng
                if (group.Type == Constants.GroupType.SINGLE)
                {
                    var us = group.Users.FirstOrDefault(x => !x.Id.Equals(userSession));
                    group.Name = us?.UserName;
                    group.Avatar = us?.Avatar;
                }

                // Lấy tin nhắn gần nhất để hiển thị
                group.LastMessage = await chatContext.Messages
                    .Where(x => x.GroupCode.Equals(group.Code))
                    .OrderByDescending(x => x.Created)
                    .Select(x => new MessageDto()
                    {
                        Created = x.Created,
                        CreatedBy = x.CreatedBy,
                        Content = x.Content,
                        GroupCode = x.GroupCode,
                        Type = x.Type,
                    })
                    .FirstOrDefaultAsync();
            }


            return groups.OrderByDescending(x => x.LastActive).ToList();
        }

        /// <summary>
        /// Tìm kiếm nhóm chat
        /// Đầu tiên sẽ lấy danh sách nhóm chat theo keySearch, sau đó lấy danh sách tên user theo keySearch
        /// Nếu user và user hiện tại đã từng nhắn tin riêng sẽ lấy Code của Group theo Group chat riêng
        /// Nếu không sẽ trả về Code của user
        /// </summary>
        /// <param name="userCode">User hiện tại đang đăng nhập</param>
        /// <param name="keySearch">Tên nhóm hoặc tên người dùng cần tìm</param>
        /// <returns></returns>
        public async Task<List<GroupDto>> SearchChatGroup(Guid userSession, string keySearch)
        {
            // Tìm kiếm danh sách group với Name Contains keySearch
            List<GroupDto> groups = await chatContext.Groups
                    .Where(x => x.GroupUsers.Any(y => y.UserCode.Equals(userSession)) && x.Name.Contains(keySearch))
                    .Select(x => new GroupDto()
                    {
                        Code = x.Code,
                        Name = x.Name,
                        Avatar = x.Avatar,
                        Type = x.Type,
                        LastActive = x.LastActive,
                        Users = x.GroupUsers.Select(y => new UserDto()
                        {
                            Id = y.User.Id,
                            UserName = y.User.UserName,
                            Avatar = y.User.Avatar,
                        }).ToList(),
                    }).ToListAsync();

            var requestContact = new GetContactRequest
            {
                KeySearch = keySearch,
                PageSize = 8
            };

            // Tìm kiếm contact theo keysearch
            var result = await _userService.GetContact(userSession, requestContact);
            List<FriendResponse> users = result.Items;
            Console.Write("User: ");

            if (users.Count > 0)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    Console.Write(users[i].Id);
                    // Lấy ra group chat riêng tư của người dùng hiện tại và contact
                    Guid? groupCode = await chatContext.Groups
                        .Where(x => x.Type.Equals(Constants.GroupType.SINGLE))
                        .Where(x => x.GroupUsers.Any(y => y.UserCode.Equals(userSession) &&
                                    x.GroupUsers.Any(y => y.UserCode.Equals(users[i].Id))))
                        .Select(x => (Guid?)x.Code)
                        .FirstOrDefaultAsync();
                    if (groupCode != null)
                    {
                        Console.WriteLine("has groupCode: ", groupCode);
                        groups.Add(new GroupDto
                        {
                            Code = groupCode,
                            Type = Constants.GroupType.SINGLE,
                            Avatar = users[i].Avatar,
                            Name = users[i].UserName,
                        });
                    }
                    else
                    {
                        Console.WriteLine("groupCode: ", groupCode);

                        groups.Add(new GroupDto
                        {
                            Code = users[i].Id,
                            Type = Constants.GroupType.SINGLE,
                            Avatar = users[i].Avatar,
                            Name = users[i].UserName,
                        });
                    }
                }
            }

            return groups;
        }

        /// <summary>
        /// - Nếu tồn tại group có code = groupCode sẽ truy cập group đó, 
        /// - Nếu không sẽ tìm kiếm user có code = groupCode và tạo nhóm chat riêng tư
        /// </summary>
        /// <param name="userCode">User hiện tại đang đăng nhập</param>
        /// <param name="groupCode">Code của group muốn truy cập</param>
        /// <returns></returns>
        public async Task<GroupDto> AccessChatGroup(Guid userCode, Guid groupCode)
        {
            try
            {
                Group grp = await chatContext.Groups
                    .FirstOrDefaultAsync(x => x.Code == groupCode);

                if (grp != null)
                {
                    var group = new GroupDto
                    {
                        Code = grp.Code,
                        Type = grp.Type,
                        Avatar = grp.Avatar,
                        Name = grp.Name,
                        Created = grp.Created,
                        CreatedBy = grp.CreatedBy,
                        LastActive = grp.LastActive,
                    };

                    if (grp.Type == GroupType.SINGLE)
                    {
                        var partner = await chatContext.GroupUsers.Where(x => x.GroupCode == groupCode && x.UserCode != userCode)
                            .Join(chatContext.Users,
                                grpUsers => grpUsers.UserCode,
                                users => users.Id,
                                (grpUsers, users) => new UserDto
                                {
                                    Id = users.Id,
                                    UserName = users.UserName,
                                    Dob = users.Dob,
                                    PhoneNumber = users.PhoneNumber,
                                    Email = users.Email,
                                    Address = users.Address,
                                    Avatar = users.Avatar,
                                    Gender = users.Gender
                                })
                            .FirstOrDefaultAsync();

                        group.Avatar = partner.Avatar;
                        group.Name = partner.UserName;
                    }
                    else
                    {
                        var groupUsers = chatContext.GroupUsers
                            .Where(x => x.GroupCode == groupCode)
                            .Join(chatContext.Users,
                                grpUsers => grpUsers.UserCode,
                                users => users.Id,
                                (grpUsers, users) => new UserDto
                                {
                                    Id = users.Id,
                                    UserName = users.UserName,
                                    Dob = users.Dob,
                                    PhoneNumber = users.PhoneNumber,
                                    Email = users.Email,
                                    Address = users.Address,
                                    Avatar = users.Avatar,
                                    Gender = users.Gender
                                })
                            .ToList();

                        group.Users = groupUsers;
                    }

                    return group;

                    //return response;
                }
                else
                {

                    Group newGroup = new Group
                    {
                        Code = Guid.NewGuid(),
                        Created = DateTime.Now,
                        CreatedBy = userCode,
                        Type = Constants.GroupType.SINGLE,
                        LastActive = DateTime.Now
                    };

                    User receiver = await chatContext.Users.FirstOrDefaultAsync(x => x.Id == groupCode);

                    newGroup.GroupUsers = new List<GroupUser>();
                    newGroup.GroupUsers.Add(new GroupUser
                    {
                        GroupCode = newGroup.Code,
                        UserCode = groupCode
                    });

                    newGroup.GroupUsers.Add(new GroupUser
                    {
                        GroupCode = newGroup.Code,
                        UserCode = userCode
                    });

                    await chatContext.Groups.AddAsync(newGroup);
                    await chatContext.SaveChangesAsync();

                    return new GroupDto
                    {
                        Code = newGroup.Code,
                        Type = newGroup.Type,
                        Avatar = receiver.Avatar,
                        Name = receiver.UserName,
                        Created = newGroup.Created,
                        CreatedBy = newGroup.CreatedBy,
                        LastActive = newGroup.LastActive,
                    };

                }
            }
            catch (Exception exception)
            {
                throw new Exception("Có lỗi xảy ra! Hãy thử lại!");
            }
        }

        /// <summary>
        /// Thông tin nhóm chat
        /// </summary>
        /// <param name="userSession">User hiện tại đang đăng nhập</param>
        /// <param name="groupCode">Mã nhóm</param>
        /// <param name="contactCode">Người chat</param>
        /// <returns></returns>
        public async Task<object> GetInfo(Guid userSession, Guid groupCode)
        {
            //Lấy thông tin nhóm chat
            Group group = await chatContext.Groups.Include(s => s.GroupUsers).ThenInclude(u => u.User).FirstOrDefaultAsync(x => x.Code.Equals(groupCode));

            if (group == null)
            {
                throw new Exception("Không tồn tại nhóm chat");
            }
            // Nếu tồn tại nhóm chat + nhóm chat có type = SINGLE (Chat 1-1) => trả về thông tin người chat cùng
            if (group.Type.Equals(Constants.GroupType.SINGLE))
            {
                var user = await chatContext.GroupUsers.FirstOrDefaultAsync(x => x.UserCode != userSession && x.GroupCode == group.Code);
                return await chatContext.Users
                        .Where(x => x.Id.Equals(user.UserCode))
                        .OrderBy(x => x.UserName)
                        .Select(x => new
                        {
                            IsGroup = false,
                            Id = x.Id,
                            Address = x.Address,
                            Avatar = x.Avatar,
                            Dob = x.Dob,
                            Email = x.Email,
                            UserName = x.UserName,
                            Gender = x.Gender,
                            PhoneNumber = x.PhoneNumber
                        })
                         .FirstOrDefaultAsync();
            }
            else
            {
                // Nếu tồn tại nhóm chat + nhóm chat nhiều người => trả về thông tin nhóm chat + thành viên trong nhóm
                return new
                {
                    IsGroup = true,
                    Code = group.Code,
                    Avatar = group.Avatar,
                    Name = group.Name,
                    Type = group.Type,
                    Users = group.GroupUsers
                        .OrderBy(x => x.User.UserName)
                        .Select(x => new UserDto()
                        {
                            Id = x.User.Id,
                            UserName = x.User.UserName,
                            Avatar = x.User.Avatar
                        }).ToList()
                };
            }
        }

        /// <summary>
        /// Thêm mới nhóm chat
        /// </summary>
        /// <param name="userCode">User hiện tại đang đăng nhập</param>
        /// <param name="group">Nhóm</param>
        public async Task AddGroup(Guid userCode, AddGroupRequest request)
        {
            DateTime dateNow = DateTime.Now;
            Group grp = new Group()
            {
                //Code = Guid.NewGuid(),
                Name = request.Name,
                Created = dateNow,
                CreatedBy = userCode,
                Type = Constants.GroupType.MULTI,
                LastActive = dateNow,
                Avatar = Constants.AVATAR_GROUP
            };

            List<GroupUser> groupUsers = request.Users.Select(x => new GroupUser()
            {
                UserCode = x.Id
            }).ToList();

            groupUsers.Add(new GroupUser()
            {
                UserCode = userCode
            });

            grp.GroupUsers = groupUsers;

            await chatContext.Groups.AddAsync(grp);
            await chatContext.SaveChangesAsync();
        }

        /// <summary>
        /// Thêm thành viên vào nhóm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task AddMembersToGroup(AddMembersToGroupRequest request)
        {
            try
            {
                var group = await chatContext.Groups.FindAsync(request.Code);

                if (group == null)
                {
                    throw new Exception("Không tìm thấy nhóm");
                }

                List<GroupUser> lstGroupUser = new List<GroupUser>();
                for (int i = 0; i < request.Users.Count; i++)
                {
                    lstGroupUser.Add(new GroupUser
                    {
                        GroupCode = request.Code,
                        UserCode = request.Users[i].Id
                    });
                }

                await chatContext.GroupUsers.AddRangeAsync(lstGroupUser);
                await chatContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra: " + ex.Message);
            }
        }

        /// <summary>
        /// Rời nhóm
        /// </summary>
        /// <param name="userSession"></param>
        /// <param name="groupCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task OutGroup(Guid userSession, Guid groupCode)
        {
            try
            {
                var groupUser = await chatContext.GroupUsers.FirstOrDefaultAsync(x => x.UserCode == userSession && x.GroupCode == groupCode);
                if (groupUser == null)
                {
                    throw new Exception("Nhóm chat không tồn tại");
                }

                chatContext.GroupUsers.Remove(groupUser);
                await chatContext.SaveChangesAsync();
            }
            catch
            {
                throw new Exception("Có lỗi xảy ra");
            }
        }

        /// <summary>
        /// Cập nhật tên nhóm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task UpdateGroupName(UpdateGroupNameRequest request)
        {
            try
            {
                Group grp = await chatContext.Groups
                    .FirstOrDefaultAsync(x => x.Code == request.Code);
                if (grp == null)
                {
                    throw new Exception("Không tìm thấy nhóm chat");
                }

                grp.Name = request.Name;
                chatContext.Groups.Update(grp);

                await chatContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra: " + ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật ảnh đại diện của nhóm chat
        /// </summary>
        /// <param name="group">Nhóm</param>
        /// <returns></returns>
        public async Task UpdateGroupAvatar(UpdateGroupAvatarRequest request)
        {
            try
            {
                Group grp = await chatContext.Groups
                    .FirstOrDefaultAsync(x => x.Code == request.Code);

                if (grp != null)
                {
                    // string path = Path.Combine(webHostEnvironment.ContentRootPath, $"/groupchat/groups/");
                    // FileHelper.CreateDirectory(path);
                    // string pathFile = path + request.Image[0].FileName;
                    // if (!File.Exists(pathFile))
                    // {

                    //     using (var stream = new FileStream(pathFile, FileMode.Create))
                    //     {
                    //         await request.Image[0].CopyToAsync(stream);
                    //         await UploadBlobFile(request.Image[0]);
                    //     }
                    // }

                    await _aswS3Service.UploadBlobFile(request.Image[0], "groupchat");
                    grp.Avatar = $"NotipCloud/groupchat/{request.Image[0].FileName}";

                    chatContext.Groups.Update(grp);
                    await chatContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra: " + ex.Message);
            }
        }

        /// <summary>
        /// Gửi tin nhắn
        /// </summary>
        /// <param name="userCode">User hiện tại đang đăng nhập</param>
        /// <param name="groupCode">Mã nhóm</param>
        /// <param name="message">Tin nhắn</param>
        public async Task SendMessage(Guid userCode, Guid groupCode, MessageDto message)
        {
            // Lấy thông tin nhóm chat
            Group grp = await chatContext.Groups.FirstOrDefaultAsync(x => x.Code.Equals(groupCode));
            DateTime dateNow = DateTime.Now;

            // Nếu nhóm không tồn tại => cố gắng lấy thông tin nhóm đã từng chat giữa 2 người
            if (grp == null)
            {
                Guid grpCode = await chatContext.Groups
                    .Where(x => x.Type.Equals(Constants.GroupType.SINGLE))
                    .Where(x => x.GroupUsers.Any(y => y.UserCode.Equals(userCode) &&
                                x.GroupUsers.Any(y => y.UserCode.Equals(message.SendTo))))
                    .Select(x => x.Code)
                    .FirstOrDefaultAsync();

                grp = await chatContext.Groups.FirstOrDefaultAsync(x => x.Code.Equals(grpCode));
            }

            // Nếu nhóm vẫn không tồn tại => tạo nhóm chat mới có 2 thành viên
            if (grp == null)
            {
                User sendTo = await chatContext.Users.FirstOrDefaultAsync(x => x.Id.Equals(message.SendTo));
                grp = new Group()
                {
                    Name = sendTo.UserName,
                    Created = dateNow,
                    CreatedBy = userCode,
                    Type = Constants.GroupType.SINGLE,
                    GroupUsers = new List<GroupUser>()
                    {
                        new GroupUser()
                        {
                            UserCode = userCode
                        },
                        new GroupUser()
                        {
                            UserCode = sendTo.Id
                        }
                    }
                };
                await chatContext.Groups.AddAsync(grp);
            }

            // Nếu tin nhắn có file => lưu file
            if (message.Attachments != null && message.Attachments.Count > 0)
            {
                string path = Path.Combine(webHostEnvironment.ContentRootPath, $"wwwroot/Attachments/{groupCode}/{DateTime.Now.Year}/");
                FileHelper.CreateDirectory(path);
                try
                {
                    if (message.Attachments[0].Length > 0)
                    {
                        string pathFile = path + message.Attachments[0].FileName;
                        if (!File.Exists(pathFile))
                        {
                            using (var stream = new FileStream(pathFile, FileMode.Create))
                            {
                                await message.Attachments[0].CopyToAsync(stream);
                            }
                        }
                        message.Path = $"Attachments/{groupCode}/{DateTime.Now.Year}/{message.Attachments[0].FileName}";
                        //message.Content = message.Attachments[0].FileName;

                        //string pathFile = $"{groupCode}/{DateTime.Now.Year}";
                        //await _aswS3Service.UploadBlobFile(message.Attachments[0], pathFile);
                        //message.Path = $"NotipCloud/{pathFile}/{message.Attachments[0].FileName}";
                        //message.Content = message.Attachments[0].FileName;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            Message msg = new Message()
            {
                Content = message.Content,
                Created = dateNow,
                CreatedBy = userCode,
                GroupCode = grp.Code,
                Path = message.Path,
                Type = message.Type,
            };

            grp.LastActive = dateNow;

            await chatContext.Messages.AddAsync(msg);
            await chatContext.SaveChangesAsync();
            try
            {
                var groupUsers = chatContext.GroupUsers
                        .Where(x => x.GroupCode == groupCode && x.UserCode != userCode)
                        .Join(chatContext.Users,
                            grpUsers => grpUsers.UserCode,
                            users => users.Id,
                            (grpUsers, users) =>
                                users.Id
                            )
                        .ToList();

                var userCreatedBy = await chatContext.Users
                    .Where(x => x.Id == userCode)
                    .Select(x => new UserDto
                    {
                        UserName = x.UserName,
                        Avatar = x.Avatar
                    })
                    .FirstOrDefaultAsync();

                var messageDto = new MessageDto
                {
                    Id = msg.Id,
                    Type = msg.Type,
                    GroupCode = msg.GroupCode,
                    Content = msg.Content,
                    Path = msg.Path,
                    Created = msg.Created,
                    CreatedBy = msg.CreatedBy,
                    UserCreatedBy = userCreatedBy
                };
                var payload = JsonConvert.SerializeObject(messageDto);
                await chatHub.SendMessage(groupUsers, payload);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách tin nhắn từ nhóm
        /// </summary>
        /// <param name="userCode">User hiện tại đang đăng nhập</param>
        /// <param name="groupCode">Mã nhóm</param>
        /// <returns>Danh sách tin nhắn</returns>
        public async Task<PagingResult<MessageDto>> GetMessageByGroup(GetMessageRequest request)
        {
            try
            {
                var query = chatContext.Messages.AsQueryable();

                if (request.groupCode != null)
                {
                    query = query.Where(x => x.GroupCode.Equals(request.groupCode));
                }

                int total = await query.CountAsync();

                if (request.PageIndex == null || request.PageIndex == 0) request.PageIndex = 1;
                if (request.PageSize == null || request.PageSize == 0) request.PageSize = total;

                int totalPages = (int)Math.Ceiling((double)total / request.PageSize.Value);

                var messages = await query
                    //.Skip((request.PageIndex.Value - 1) * request.PageSize.Value)
                    //.Take(request.PageSize.Value)
                    .OrderBy(x => x.Created)
                    .Select(x => new MessageDto()
                    {
                        Created = x.Created,
                        Content = x.Content,
                        CreatedBy = x.CreatedBy,
                        GroupCode = x.GroupCode,
                        Id = x.Id,
                        Path = x.Path,
                        Type = x.Type,
                        UserCreatedBy = new UserDto()
                        {
                            Id = x.UserCreatedBy.Id,
                            UserName = x.UserCreatedBy.UserName,
                            Dob = x.UserCreatedBy.Dob,
                            PhoneNumber = x.UserCreatedBy.PhoneNumber,
                            Email = x.UserCreatedBy.Email,
                            Address = x.UserCreatedBy.Address,
                            Gender = x.UserCreatedBy.Gender,
                            Avatar = x.UserCreatedBy.Avatar

                        }
                    }).ToListAsync();

                return new PagingResult<MessageDto>(messages, request.PageIndex.Value, request.PageSize.Value, total, totalPages);
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra!");
            }
        }

        /// <summary>
        /// Cập nhật ảnh phòng chat
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<GroupDto> UpdatePhotoChat(UpdateGroupAvatarRequest request)
        {
            var group = await chatContext.Groups.FirstOrDefaultAsync(x => x.Code == request.Code);
            if (group == null)
            {
                throw new Exception("Có lỗi xảy ra!");
            }
            string path = Path.Combine(webHostEnvironment.ContentRootPath, $"wwwroot/PhotoChat/{request.Code}/");
            FileHelper.CreateDirectory(path);
            try
            {
                if (request.Image[0].Length > 0)
                {
                    string pathFile = path + request.Image[0].FileName;
                    if (!File.Exists(pathFile))
                    {
                        using (var stream = new FileStream(pathFile, FileMode.Create))
                        {
                            await request.Image[0].CopyToAsync(stream);
                        }
                    }
                    group.Avatar = $"PhotoChat/{request.Code}/{request.Image[0].FileName}";

                    chatContext.Groups.Update(group);
                    await chatContext.SaveChangesAsync();
                    return new GroupDto
                    {
                        Avatar = group.Avatar
                    };
                    //string pathFile = $"{groupCode}/{DateTime.Now.Year}";
                    //await _aswS3Service.UploadBlobFile(message.Attachments[0], pathFile);
                    //message.Path = $"NotipCloud/{pathFile}/{message.Attachments[0].FileName}";
                    //message.Content = message.Attachments[0].FileName;
                }

                else
                {
                    throw new Exception("Không tìm thấy hình ảnh!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra!");
            }
        }

        #region admin

        /// <summary>
        /// Lấy tất cả phòng chat
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<PagingResult<GroupAdminResponse>> GetAllChatRoom(PagingRequest request)
        {
            try
            {
                var query = chatContext.Groups.Include(m => m.Messages).Include(g => g.GroupUsers).AsQueryable();

                int total = await query.CountAsync();

                if (request.PageIndex == null || request.PageIndex == 0) request.PageIndex = 1;
                if (request.PageSize == null || request.PageSize == 0) request.PageSize = total;

                int totalPages = (int)Math.Ceiling((double)total / request.PageSize.Value);


                var groups = await query
                    .Skip((request.PageIndex.Value - 1) * request.PageSize.Value)
                    .Take(request.PageSize.Value)
                    .Select(x => new GroupAdminResponse()
                    {
                        Code = x.Code,
                        Type = x.Type,
                        Avatar = "images/no_image.jpg",
                        Name = x.Type == Constants.GroupType.SINGLE ? String.Join(", ", x.GroupUsers.Select(x => x.User.UserName)) : x.Name,
                        Created = x.Created,
                        CreatedBy = x.CreatedBy,
                        LastActive = x.LastActive,
                        NumberOfMessage = x.Messages.Count,
                        NumberOfMember = x.GroupUsers.Count
                    })
                    .ToListAsync();

                return new PagingResult<GroupAdminResponse>(groups, request.PageIndex.Value, request.PageSize.Value, total, totalPages);
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra!");
            }
        }

        public async Task<GroupAdminResponse> GetDetailChatRoom(Guid groupCode)
        {
            try
            {
                var group = await chatContext.Groups
                    .Where(x => x.Code == groupCode)
                    .Select(x => new GroupAdminResponse()
                    {
                        Code = x.Code,
                        Type = x.Type,
                        Avatar = "images/no_image.jpg",
                        Name = x.Type == Constants.GroupType.SINGLE ? String.Join(", ", x.GroupUsers.Select(x => x.User.UserName)) : x.Name,
                        Created = x.Created,
                        CreatedBy = x.CreatedBy,
                        LastActive = x.LastActive,
                        NumberOfMessage = x.Messages.Count,
                        NumberOfMember = x.GroupUsers.Count
                    })
                    .FirstOrDefaultAsync();

                if (group != null)
                {
                    var message = await GetMessageByGroup(new GetMessageRequest { groupCode = groupCode });
                    group.Messages = message.Items;

                    var groupUsers = chatContext.GroupUsers
                            .Where(x => x.GroupCode == groupCode)
                            .Join(chatContext.Users,
                                grpUsers => grpUsers.UserCode,
                                users => users.Id,
                                (grpUsers, users) => new UserDto
                                {
                                    Id = users.Id,
                                    UserName = users.UserName,
                                    Dob = users.Dob,
                                    PhoneNumber = users.PhoneNumber,
                                    Email = users.Email,
                                    Address = users.Address,
                                    Avatar = users.Avatar,
                                    Gender = users.Gender
                                })
                            .ToList();
                    group.Users = groupUsers;

                    return group;
                }
                else
                {
                    throw new Exception("Có lỗi xảy ra!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra!");
            }
        }

        public async Task<List<MessageDto>> GetAllMessages()
        {
            try
            {
                var messages = await chatContext.Messages
                    .Select(x => new MessageDto()
                    {
                        Created = x.Created,
                        Content = x.Content,
                        CreatedBy = x.CreatedBy,
                        GroupCode = x.GroupCode,
                        Id = x.Id,
                        Path = x.Path,
                        Type = x.Type,
                        UserCreatedBy = new UserDto()
                        {
                            Id = x.UserCreatedBy.Id,
                            UserName = x.UserCreatedBy.UserName,
                            Dob = x.UserCreatedBy.Dob,
                            PhoneNumber = x.UserCreatedBy.PhoneNumber,
                            Email = x.UserCreatedBy.Email,
                            Address = x.UserCreatedBy.Address,
                            Gender = x.UserCreatedBy.Gender,
                            Avatar = x.UserCreatedBy.Avatar

                        }
                    })
                    .ToListAsync();
                return messages;
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra!");
            }
        }

        public async Task<List<TrafficStatisticsResult>> TrafficStatistics(TrafficStatisticsRequest request)
        {
            try
            {
                var messageStatistics = await chatContext.TrafficStatisticsResult
                    .FromSqlRaw("CALL GetStatisticsMessageUser({0}, {1})", request.StatisticByYear ?? 0, request.StatisticByMonth ?? 0)
                    .ToListAsync();

                if(messageStatistics != null)
                {
                    messageStatistics = messageStatistics.OrderBy(x => x.Month).ToList();
                    if (messageStatistics[0].Day != null)
                    {
                        messageStatistics = messageStatistics.OrderBy(x => x.Day).ToList();
                    }
                }

                return messageStatistics;
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra!");
            }
        }
        #endregion
    }
}
