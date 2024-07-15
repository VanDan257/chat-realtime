namespace notip_server.Models
{
    public class AttributesShop
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public virtual Shop Shop { get; set; }
    }
}
