using notip_server.ViewModel.Common;
using notip_server.ViewModel.Friend;
using notip_server.ViewModel.User;

namespace notip_server.Interfaces
{
    public interface IFriendService
    {
        Task<PagingResult<FriendResponse>> GetListFriend(Guid userSession, GetContactRequest request);

        Task<PagingResult<FriendResponse>> GetListFriendInvite(Guid userSession, GetContactRequest request);

        Task SendFriendRequest(Guid userSession, Guid receiverCode);

        Task AcceptFriendRequest(Guid userSession, Guid receiverCode);

        Task CancelFriendRequest(Guid userSession, Guid receiverCode);

        Task BlockUser(Guid userSession, Guid receiverCode);

        Task UnBlockUser(Guid userSession, Guid receiverCode);
    }
}
