using notip_server.Models;
using notip_server.ViewModel.Friend;

namespace notip_server.Dto
{
    public class GroupDto
    {
        public Guid? Code { get; set; }

        public string Type { get; set; }

        public string Avatar { get; set; }

        public string Name { get; set; }

        public DateTime Created { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime LastActive { get; set; }

        public List<UserDto> Users { get; set; }

        public MessageDto LastMessage { get; set; }
    }
}
