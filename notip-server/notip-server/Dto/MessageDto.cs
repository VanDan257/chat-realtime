using notip_server.Models;

namespace notip_server.Dto
{
    public class MessageDto
    {
        public long Id { get; set; }

        public string Type { get; set; }

        public Guid GroupCode { get; set; }

        public string Content { get; set; }

        public string Path { get; set; }

        public DateTime Created { get; set; }

        public Guid CreatedBy { get; set; }

        public string SendTo { get; set; }

        public UserDto UserCreatedBy { get; set; }

        public List<IFormFile> Attachments { get; set; }
    }
}
