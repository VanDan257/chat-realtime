namespace notip_server.Dto
{
    public class AccessToken
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string Token { get; set; }
    }
}
