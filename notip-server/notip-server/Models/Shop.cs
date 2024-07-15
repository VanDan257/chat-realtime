using Microsoft.AspNetCore.Http.HttpResults;

namespace notip_server.Models
{
    public class Shop
    {
        public Guid Id { get; set; }
        public string Name{ get; set; }
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public int Start { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public virtual ICollection<AttributesShop> Attributes { get; set; }
    }
}
