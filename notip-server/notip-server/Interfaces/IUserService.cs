using notip_server.Dto;
using notip_server.Models;
using notip_server.ViewModel.Common;
using notip_server.ViewModel.Friend;
using notip_server.ViewModel.User;

namespace notip_server.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetProfile(Guid userCode);
        Task<UserDto> UpdateProfile(Guid userCode, UpdateProfileRequest request);
        Task<UserDto> UpdateAvatar(Guid userCode, UpdateAvatarRequest request);
        Task<PagingResult<FriendResponse>> GetContact(Guid userSession, GetContactRequest request);

        #region Admin

        Task<PagingResult<ResponseUserAdminHome>> GetAllUser(PagingRequest request);
        Task<PagingResult<UserDto>> GetStaffs(PagingRequest request);
        Task CreateStaff(CreateAdminRequest request);
        #endregion
    }
}
