using notip_server.Dto;
using notip_server.Models;

namespace notip_server.ViewModel.ChatBoard
{
    public class GroupAdminResponse
    {
        public Guid Code { get; set; }

        public string Type { get; set; }

        public string Avatar { get; set; }

        public string Name { get; set; }

        public DateTime Created { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime LastActive { get; set; }

        public int NumberOfMessage { get; set; }

        public int NumberOfMember { get; set; }

        public ICollection<MessageDto> Messages { get; set; }
        public ICollection<UserDto> Users { get; set; }
    }
}
