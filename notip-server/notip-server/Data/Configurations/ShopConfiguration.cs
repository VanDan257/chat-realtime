using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using notip_server.Models;

namespace notip_server.Data.Configurations
{
    public class ShopConfiguration : IEntityTypeConfiguration<Shop>
    {
        public void Configure(EntityTypeBuilder<Shop> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.Attributes).WithOne(x => x.Shop)
                .HasForeignKey(x => x.Shop)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
