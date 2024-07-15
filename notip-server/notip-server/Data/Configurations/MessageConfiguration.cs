using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using notip_server.Models;

namespace notip_server.Data.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Message");

            builder.Property(e => e.Created).HasColumnType("datetime");
            builder.Property(e => e.CreatedBy)
                .HasMaxLength(32)
                .IsUnicode(false);
            builder.Property(e => e.GroupCode)
                .HasMaxLength(32)
                .IsUnicode(false);
            builder.Property(e => e.Path).HasMaxLength(255);
            builder.Property(e => e.Type)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("text\r\nmedia\r\nattachment");

            builder.HasOne(d => d.UserCreatedBy).WithMany(p => p.Messages)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Message_User");

            builder.HasOne(d => d.Group).WithMany(p => p.Messages)
                .HasForeignKey(d => d.GroupCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Message_Group");
        }
    }
}
