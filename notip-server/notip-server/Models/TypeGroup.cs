namespace notip_server.Models
{
    public class TypeGroup
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
    }
}
