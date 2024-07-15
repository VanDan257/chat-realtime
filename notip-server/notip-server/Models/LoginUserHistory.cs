namespace notip_server.Models
{
    public class LoginUserHistory
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime LoginTime { get; set; }

        public virtual User User { get; set; }
    }
}
