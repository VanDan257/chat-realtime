using notip_server.Dto;
using notip_server.Models;

namespace notip_server.ViewModel.User
{
    public class ResponseUserAdminHome
    {
        public UserDto User { get; set; }
        public int MessageCount { get; set; }
    }
}
