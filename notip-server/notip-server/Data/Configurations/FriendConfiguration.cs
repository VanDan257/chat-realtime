using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using notip_server.Models;

namespace notip_server.Data.Configurations
{
    public class FriendConfiguration : IEntityTypeConfiguration<Friend>
    {
        public void Configure(EntityTypeBuilder<Friend> builder)
        {
            builder.ToTable("Friend");

            builder.HasKey(x => x.Id);

            builder.Property(e => e.ReceiverCode)
                .HasMaxLength(32)
                .IsUnicode(false);
            builder.Property(e => e.SenderCode)
                .HasMaxLength(32)
                .IsUnicode(false);
            builder.Property(e => e.Status)
                .HasMaxLength(32);
        }
    }
}
