namespace notip_server.Dto
{
    public class GroupCallDto
    {
        public Guid Code { get; set; }
        public string Type { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime LastActive { get; set; }

        public List<CallDto> Calls { get; set; }
        public CallDto LastCall { get; set; }

    }
}
