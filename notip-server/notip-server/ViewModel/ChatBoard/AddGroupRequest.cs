using notip_server.Dto;

namespace notip_server.ViewModel.ChatBoard
{
    public class AddGroupRequest
    {
        public string Name { get; set; }
        public List<UserDto> Users { get; set; }
    }
}
