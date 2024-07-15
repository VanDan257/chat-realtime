using Azure.Core;
using notip_server.Dto;
using notip_server.Models;
using notip_server.ViewModel.ChatBoard;
using notip_server.ViewModel.Common;

namespace notip_server.Interfaces
{
    public interface IChatBoardService
    {
        Task<List<GroupDto>> GetHistory(Guid userSession);
        Task<List<GroupDto>> SearchChatGroup(Guid userSession, string keySearch);
        Task<GroupDto> AccessChatGroup(Guid userSession, Guid groupCode);
        Task<object> GetInfo(Guid userSession, Guid groupCode);
        Task AddGroup(Guid userSession, AddGroupRequest request);
        Task AddMembersToGroup(AddMembersToGroupRequest request);
        Task UpdateGroupName(UpdateGroupNameRequest request);
        Task OutGroup(Guid userSession, Guid groupCode);
        Task UpdateGroupAvatar(UpdateGroupAvatarRequest request);
        Task SendMessage(Guid userSession, Guid groupCode, MessageDto message);
        Task<PagingResult<MessageDto>> GetMessageByGroup(GetMessageRequest request);
        Task<PagingResult<GroupAdminResponse>> GetAllChatRoom(PagingRequest request);

        #region Admin
        Task<GroupDto> UpdatePhotoChat(UpdateGroupAvatarRequest request);
        Task<GroupAdminResponse> GetDetailChatRoom(Guid groupCode);
        Task<List<MessageDto>> GetAllMessages();
        Task<List<TrafficStatisticsResult>> TrafficStatistics(TrafficStatisticsRequest request);
        #endregion
    }
}
