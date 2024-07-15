namespace notip_server.Models
{
    public class GroupUser
    {
        public long Id { get; set; }

        public Guid GroupCode { get; set; }

        public Guid UserCode { get; set; }

        public virtual Group Group { get; set; }

        public virtual User User { get; set; }
    }
}
