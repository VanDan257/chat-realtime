namespace notip_server.Dto
{
    public class CallDto
    {
        public int Id { get; set; }
        public Guid GroupCallCode { get; set; }
        public Guid UserCode { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public DateTime Created { get; set; }

        public UserDto User { get; set; }
    }
}
