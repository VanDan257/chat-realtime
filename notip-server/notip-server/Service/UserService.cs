using Microsoft.EntityFrameworkCore;
using notip_server.Data;
using notip_server.Dto;
using notip_server.Models;
using notip_server.Interfaces;
using notip_server.ViewModel.User;
using System.Numerics;
using System.Net.WebSockets;
using notip_server.Utils;
using notip_server.ViewModel.Friend;
using notip_server.ViewModel.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using System.Runtime.ExceptionServices;
using static notip_server.Utils.Constants;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace notip_server.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DbChatContext chatContext;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IPasswordService _passwordService;
        public UserService(DbChatContext chatContext, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, IPasswordService passwordService, IWebHostEnvironment webHostEnvironment)
        {
            this.chatContext = chatContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _passwordService = passwordService;
            this.webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Lấy thông tin cá nhân của user
        /// </summary>
        /// <param name="userCode">User hiện tại đang đăng nhập</param>
        /// <returns>Thông tin user</returns>
        public async Task<UserDto> GetProfile(Guid userCode)
        {
            return await chatContext.Users
                    .Where(x => x.Id.Equals(userCode))
                    .Select(x => new UserDto()
                    {
                        Id = x.Id,
                        UserName = x.UserName,
                        Address = x.Address,
                        Avatar = x.Avatar,
                        Email = x.Email,
                        Gender = x.Gender,
                        PhoneNumber = x.PhoneNumber,
                        Dob = x.Dob
                    }).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Cập nhật thông tin cá nhân
        /// </summary>
        /// <param name="userCode">User hiện tại đang đăng nhập</param>
        /// <param name="user">Thông tin user</param>
        /// <returns></returns>
        public async Task<UserDto> UpdateProfile(Guid userCode, UpdateProfileRequest request)
        {
            User us = await chatContext.Users
                    .FirstOrDefaultAsync(x => x.Id.Equals(userCode));
            
            if (us != null)
            {
                us.UserName = request.UserName;
                us.Dob = request.Dob;
                us.Address = request.Address;
                us.PhoneNumber = request.PhoneNumber;
                us.Gender = request.Gender;

                await chatContext.SaveChangesAsync();
            }

            return new UserDto
            {
                Id = us.Id,   
                UserName = us.UserName,
                Dob = us.Dob,
                PhoneNumber = us.PhoneNumber,
                Gender = us.Gender,
                Email = us.Email,
                Address = us.Address,
                Avatar = us.Avatar,
            };
        }

        /// <summary>
        /// Lấy thông tin người dùng theo code
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public async Task<PagingResult<FriendResponse>> GetContact(Guid userSession, GetContactRequest request)
        {
            try
            {
                var query = chatContext.Users.AsQueryable();

                if (!string.IsNullOrEmpty(request.UserCode.ToString()))
                {
                    query = query.Where(x => x.Id == request.UserCode);
                }
                else if (!string.IsNullOrEmpty(request.KeySearch))
                {
                    query = query.Where(x => x.Id != userSession)
                                 .Where(x => x.UserName.ToLower().Contains(request.KeySearch.ToLower()) || x.Email.ToLower().Contains(request.KeySearch.ToLower()) || x.PhoneNumber.Contains(request.KeySearch));
                }

                int total = await query.CountAsync();

                if (request.PageIndex == null || request.PageIndex == 0) request.PageIndex = 1;
                if (request.PageSize == null || request.PageSize == 0) request.PageSize = total;

                int totalPages = (int)Math.Ceiling((double)total / request.PageSize.Value);

                var users = await query
                    .Skip((request.PageIndex.Value - 1) * request.PageSize.Value)
                    .Take(request.PageSize.Value)
                    .Select(x => new FriendResponse()
                    {
                        Avatar = x.Avatar,
                        Id = x.Id,
                        UserName = x.UserName,
                        Address = x.Address,
                        Dob = x.Dob,
                        Email = x.Email,
                        Gender = x.Gender,
                        PhoneNumber = x.PhoneNumber
                    })
                    .OrderBy(x => x.UserName)
                    .ToListAsync();

                if (users != null)
                {
                    foreach(var user in users)
                    {
                        var friend = await chatContext.Friends.FirstOrDefaultAsync(x => x.SenderCode == userSession && x.ReceiverCode == user.Id);
                        if(friend != null)
                        {
                            switch (friend.Status)
                            {
                                case Constants.FriendStatus.FRIEND:
                                    user.IsFriend = true;
                                    break;

                                case Constants.FriendStatus.FRIENDREQUEST:
                                    user.IsSentFriend = true;
                                    break;

                                case Constants.FriendStatus.BLOCKED:
                                    user.IsBlocked = true;
                                    break;
                            }
                        }
                        else
                        {
                            var friend1 = await chatContext.Friends.FirstOrDefaultAsync(x => x.SenderCode == user.Id && x.ReceiverCode == userSession);
                            if (friend1 != null)
                            {
                                switch (friend1.Status)
                                {
                                    case Constants.FriendStatus.FRIEND:
                                        user.IsFriend = true;
                                        break;

                                    case Constants.FriendStatus.FRIENDREQUEST:
                                        user.IsBeenSentFriend = true;
                                        break;

                                    case Constants.FriendStatus.BLOCKED:
                                        user.IsBeenBlocked = true;
                                        break;
                                }
                            }
                        }
                    }

                    return new PagingResult<FriendResponse>(users, request.PageIndex.Value, request.PageSize.Value, total, totalPages);
                }
                else
                {
                    throw new Exception("Không tìm thấy người dùng");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật ảnh đại diện người dùng
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<UserDto> UpdateAvatar(Guid userCode, UpdateAvatarRequest request)
        {
            var user = await chatContext.Users.FirstOrDefaultAsync(x => x.Id == userCode);
            if(user == null)
            {
                throw new Exception("Có lỗi xảy ra!");
            }
            string path = Path.Combine(webHostEnvironment.ContentRootPath, $"wwwroot/Avatar/{userCode}/");
            
            try
            {
                if (request.Image[0].Length > 0)
                {
                    string pathFile = path + request.Image[0].FileName;
                    FileHelper.CreateDirectory(path);
                    if (!File.Exists(pathFile))
                    {
                        using (var stream = new FileStream(pathFile, FileMode.Create))
                        {
                            await request.Image[0].CopyToAsync(stream);
                        }
                    }
                    user.Avatar= $"Avatar/{userCode}/{request.Image[0].FileName}";

                    chatContext.Users.Update(user);
                    await chatContext.SaveChangesAsync();

                    //string pathFile = $"{groupCode}/{DateTime.Now.Year}";
                    //await _aswS3Service.UploadBlobFile(message.Attachments[0], pathFile);
                    //message.Path = $"NotipCloud/{pathFile}/{message.Attachments[0].FileName}";
                    //message.Content = message.Attachments[0].FileName;
                    return new UserDto
                    {
                        Avatar = user.Avatar
                    };
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

        #region Admin

        /// <summary>
        /// Lấy danh sách người dùng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<PagingResult<ResponseUserAdminHome>> GetAllUser(PagingRequest request)
        {
            try
            {

                var query = chatContext.Users
                    //.Include(m => m.Messages)
                    .OrderByDescending(u => u.Messages.Count)
                    .Select(user => new ResponseUserAdminHome
                    {
                        User = new UserDto
                        {
                            UserName = user.UserName,
                            Dob = user.Dob,
                            PhoneNumber = user.PhoneNumber,
                            Email = user.Email,
                            Address = user.Address,
                            Avatar = user.Avatar,
                            Gender = user.Gender,
                            Created = user.Created,
                            LastLogin = user.LastLogin
                        },
                        MessageCount = chatContext.Messages.Count(message => message.CreatedBy == user.Id)
                    })
                    .AsQueryable();

                int total = await query.CountAsync();

                if (request.PageIndex == null || request.PageIndex == 0) request.PageIndex = 1;
                if (request.PageSize == null || request.PageSize == 0) request.PageSize = total;

                int totalPages = (int)Math.Ceiling((double)total / request.PageSize.Value);

                var users = await query
                    .Skip((request.PageIndex.Value - 1) * request.PageSize.Value)
                    .Take(request.PageSize.Value)
                    .ToListAsync();

                return new PagingResult<ResponseUserAdminHome>(users, request.PageIndex.Value, request.PageSize.Value, total, totalPages);
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra!");
            }
        }

        /// <summary>
        /// Lấy danh sách admin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<PagingResult<UserDto>> GetStaffs(PagingRequest request)
        {
            try
            {
                var query = chatContext.Roles
                    .Where(x => x.NormalizedName == Constants.Role.ADMIN)
                    .Join(chatContext.UserRoles,
                    role => role.Id,
                    userRole => userRole.RoleId,
                    (rolee, userRole) => userRole.UserId)
                    .AsQueryable();

                int total = await query.CountAsync();

                if (request.PageIndex == null || request.PageIndex == 0) request.PageIndex = 1;
                if (request.PageSize == null || request.PageSize == 0) request.PageSize = total;

                int totalPages = (int)Math.Ceiling((double)total / request.PageSize.Value);

                var staffIds = await query
                    .Skip((request.PageIndex.Value - 1) * request.PageSize.Value)
                    .Take(request.PageSize.Value)
                    .ToListAsync();

                List<UserDto> usersDto = new List<UserDto>();

                foreach(var staffId in staffIds)
                {
                    var user = await chatContext.Users.Where(x => x.Id == staffId)
                        .Select(user => new UserDto
                        {
                            UserName = user.UserName,
                            Dob = user.Dob,
                            PhoneNumber = user.PhoneNumber,
                            Email = user.Email,
                            Address = user.Address,
                            Avatar = user.Avatar,
                            Gender = user.Gender,
                            Created = user.Created,
                            LastLogin = user.LastLogin
                        })
                        .FirstOrDefaultAsync();

                    if(user != null)
                    {
                        usersDto.Add(user);
                    }
                }
                return new PagingResult<UserDto>(usersDto, request.PageIndex.Value, request.PageSize.Value, total, totalPages);
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra");
            }
        }

        public async Task CreateStaff(CreateAdminRequest request)
        {
            try
            {
                if (await chatContext.Users.AnyAsync(x => x.Email.Equals(request.Email)))
                    throw new Exception("Email đã tồn tại");

                var saltPassword = _passwordService.GenerateSalt();
                var hashPassword = _passwordService.HashPassword(request.Password, saltPassword);

                var guid = Guid.NewGuid();

                string path = Path.Combine(webHostEnvironment.ContentRootPath, $"wwwroot/Avatar/{guid}/");

                User newUser = new User()
                {
                    Id = guid,
                    UserName = request.UserName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    PasswordSalt = saltPassword,
                    PasswordHash = hashPassword,
                    Avatar = Constants.AVATAR_DEFAULT,
                    NormalizedEmail = request.Email.ToLower(),
                    NormalizedUserName = request.UserName.ToLower(),
                };

                var role = await chatContext.Roles.FirstOrDefaultAsync(x => x.NormalizedName == request.Role);
                UserRole userRole = new UserRole();
                if (role != null)
                {
                    userRole.UserId = newUser.Id;
                    userRole.RoleId = role.Id;
                }

                if (request.Avatar[0].Length > 0)
                {
                    string pathFile = path + request.Avatar[0].FileName;
                    FileHelper.CreateDirectory(path);
                    if (!File.Exists(pathFile))
                    {
                        using (var stream = new FileStream(pathFile, FileMode.Create))
                        {
                            await request.Avatar[0].CopyToAsync(stream);
                        }
                    }
                    newUser.Avatar = $"Avatar/{guid}/{request.Avatar[0].FileName}";

                }

                await chatContext.AddAsync(newUser);
                await chatContext.AddAsync(userRole);
                await chatContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra!");
            }
        }
        #endregion
    }
}
