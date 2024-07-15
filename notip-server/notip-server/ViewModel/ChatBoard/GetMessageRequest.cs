using notip_server.ViewModel.Common;

namespace notip_server.ViewModel.ChatBoard
{
    public class GetMessageRequest : PagingRequest
    {
        public Guid? groupCode { get; set; }
    }
}
