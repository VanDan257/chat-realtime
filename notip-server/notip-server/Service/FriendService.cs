using Microsoft.EntityFrameworkCore;
using notip_server.Data;
using notip_server.Interfaces;
using notip_server.Models;
using notip_server.Utils;
using notip_server.ViewModel.Common;
using notip_server.ViewModel.Friend;
using notip_server.ViewModel.User;

namespace notip_server.Service
{
    public class FriendService : IFriendService
    {
        private readonly DbChatContext _chatContext;

        public FriendService(DbChatContext chatContext)
        {
            _chatContext = chatContext;
        }

        /// <summary>
        /// Lấy danh sách contact là bạn bè với user hiện tại
        /// </summary>
        /// <param name="userSession"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PagingResult<FriendResponse>> GetListFriend(Guid userSession, GetContactRequest request)
        {
            var listFriend1 = await _chatContext.Friends.Where(x => x.SenderCode == userSession && x.Status == Constants.FriendStatus.FRIEND)
                .Join(_chatContext.Users,
                    friend => friend.ReceiverCode,
                    user => user.Id,
                    (friend, user) => new FriendResponse()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Dob = user.Dob,
                        PhoneNumber = user.PhoneNumber,
                        Email = user.Email,
                        Address = user.Address,
                        Avatar = user.Avatar,
                        IsFriend = true
                    })
                .ToListAsync();
            var listFriend2 = await _chatContext.Friends.Where(x => x.ReceiverCode == userSession && x.Status == Constants.FriendStatus.FRIEND)
                .Join(_chatContext.Users,
                    friend => friend.SenderCode,
                    user => user.Id,
                    (friend, user) => new FriendResponse()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Dob = user.Dob,
                        PhoneNumber = user.PhoneNumber,
                        Email = user.Email,
                        Address = user.Address,
                        Avatar = user.Avatar,
                        IsFriend = true
                    })
                .ToListAsync();
            var friends = listFriend1.Concat(listFriend2).ToList();

            int total = friends.Count();

            if (request.PageIndex == null || request.PageIndex == 0) request.PageIndex = 1;
            if (request.PageSize == null || request.PageSize == 0) request.PageSize = total;

            int totalPages = (int)Math.Ceiling((double)total / request.PageSize.Value);

            friends = friends
                .Skip((request.PageIndex.Value - 1) * request.PageSize.Value)
                .Take(request.PageSize.Value)
                .OrderBy(x => x.UserName)
                .ToList();

            return new PagingResult<FriendResponse>(friends, request.PageIndex.Value, request.PageSize.Value, total, totalPages);
        }

        /// <summary>
        /// Lấy danh sách contact gửi lời mời kết bạn cho người dùng hiện tại
        /// </summary>
        /// <param name="userSession"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PagingResult<FriendResponse>> GetListFriendInvite(Guid userSession, GetContactRequest request)
        {
            var query = from user in _chatContext.Users
                        join friend in _chatContext.Friends
                        on user.Id equals friend.SenderCode
                        where friend.ReceiverCode == userSession
                                && friend.Status == Constants.FriendStatus.FRIENDREQUEST
                        select new FriendResponse
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            Dob = user.Dob,
                            PhoneNumber = user.PhoneNumber,
                            Email = user.Email,
                            Address = user.Address,
                            Avatar = user.Avatar,
                            IsBeenSentFriend = true
                        };
            int total = await query.CountAsync();

            if (request.PageIndex == null || request.PageIndex == 0) request.PageIndex = 1;
            if (request.PageSize == null || request.PageSize == 0) request.PageSize = total;

            int totalPages = (int)Math.Ceiling((double)total / request.PageSize.Value);

            var friends = await query
                .Skip((request.PageIndex.Value - 1) * request.PageSize.Value)
                .Take(request.PageSize.Value)
                .OrderBy(x => x.UserName)
            .ToListAsync();

            return new PagingResult<FriendResponse>(friends, request.PageIndex.Value, request.PageSize.Value, total, totalPages);
        }

        /// <summary>
        /// Gửi yêu cầu kết bạn
        /// </summary>
        /// <param name="userSession"></param>
        /// <param name="receiverCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task SendFriendRequest(Guid userSession, Guid receiverCode)
        {
            try
            {
                var receiver = await _chatContext.Users.FirstOrDefaultAsync(x => x.Id == receiverCode);

                if(receiver != null)
                {
                    await _chatContext.Friends.AddAsync(new Models.Friend
                    {
                        SenderCode = userSession,
                        ReceiverCode = receiverCode,
                        Status = Constants.FriendStatus.FRIENDREQUEST
                    });

                    await _chatContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Không tìm thấy người dùng!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Chấp nhận lời mời kết bạn
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task AcceptFriendRequest(Guid userSession, Guid receiverCode)
        {
            try
            {
                var friend = await _chatContext.Friends.FirstOrDefaultAsync(x => x.SenderCode == userSession && x.ReceiverCode == receiverCode);
                if (friend != null)
                {
                    friend.Status = Constants.FriendStatus.FRIEND;
                    _chatContext.Friends.Update(friend);

                    await _chatContext.SaveChangesAsync();
                }
                else
                {
                    var friend1 = await _chatContext.Friends.FirstOrDefaultAsync(x => x.SenderCode == receiverCode && x.ReceiverCode == userSession);
                    if(friend1 != null)
                    {

                        friend1.Status = Constants.FriendStatus.FRIEND;
                        _chatContext.Friends.Update(friend1);

                        await _chatContext.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("Có lỗi xảy ra!");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Huỷ yêu cầu kết bạn
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task CancelFriendRequest(Guid userSession, Guid receiverCode)
        {
            try
            {
                var friend = await _chatContext.Friends.FirstOrDefaultAsync(x => x.SenderCode == userSession && x.ReceiverCode == receiverCode);
                if (friend != null)
                {
                    _chatContext.Remove(friend);

                    await _chatContext.SaveChangesAsync();
                }
                else
                {
                    var friend1 = await _chatContext.Friends.FirstOrDefaultAsync(x => x.SenderCode == receiverCode && x.ReceiverCode == userSession);
                    if (friend1 != null)
                    {
                        _chatContext.Remove(friend1);

                        await _chatContext.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("Có lỗi xảy ra!");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Chặn người dùng
        /// </summary>
        /// <param name="userSession"></param>
        /// <param name="receiverCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task BlockUser(Guid userSession, Guid receiverCode)
        {
            try
            {
                var receiver = await _chatContext.Users.FirstOrDefaultAsync(x => x.Id == receiverCode);

                if (receiver != null)
                {
                    await _chatContext.Friends.AddAsync(new Models.Friend
                    {
                        SenderCode = userSession,
                        ReceiverCode = receiverCode,
                        Status = Constants.FriendStatus.BLOCKED
                    });

                    await _chatContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Không tìm thấy người dùng!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Bỏ chặn người dùng
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task UnBlockUser(Guid userSession, Guid receiverCode)
        {
            try
            {
                var friend = await _chatContext.Friends.FirstOrDefaultAsync(x => x.SenderCode == userSession && x.ReceiverCode == receiverCode);
                if (friend != null)
                {
                    _chatContext.Remove(friend);

                    await _chatContext.SaveChangesAsync();
                }
                else
                {
                    var friend1 = await _chatContext.Friends.FirstOrDefaultAsync(x => x.SenderCode == receiverCode && x.ReceiverCode == userSession);
                    if (friend1 != null)
                    {
                        _chatContext.Remove(friend);

                        await _chatContext.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("Có lỗi xảy ra!");
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
