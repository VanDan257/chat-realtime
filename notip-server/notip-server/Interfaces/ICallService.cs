using notip_server.Dto;

namespace notip_server.Interfaces
{
    public interface ICallService
    {
        Task<List<GroupCallDto>> GetCallHistory(Guid userSession);
        Task<List<CallDto>> GetHistoryById(Guid userSession, Guid groupCallCode);
        Task<string> Call(Guid userSession, Guid callTo);
        Task JoinVideoCall(Guid userSession, string url);
        Task CancelVideoCall(Guid userSession, string url);

    }
}
