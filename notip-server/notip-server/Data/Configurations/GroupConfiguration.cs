using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using notip_server.Models;

namespace notip_server.Data.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.HasKey(e => e.Code);

            builder.ToTable("Group");

            builder.Property(e => e.Code)
                .HasMaxLength(32)
                .IsUnicode(false);
            builder.Property(e => e.Avatar).IsUnicode(false);
            builder.Property(e => e.Created).HasColumnType("datetime");
            builder.Property(e => e.CreatedBy)
                .HasMaxLength(32)
                .IsUnicode(false);
            builder.Property(e => e.LastActive).HasColumnType("datetime");
            builder.Property(e => e.Name).HasMaxLength(250);
            builder.Property(e => e.Type)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasComment("single: chat 1-1\r\nmulti: chat 1-n");

        }
    }
}
