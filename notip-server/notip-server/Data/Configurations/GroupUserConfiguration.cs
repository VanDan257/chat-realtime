using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using notip_server.Models;

namespace notip_server.Data.Configurations
{
    public class GroupUserConfiguration : IEntityTypeConfiguration<GroupUser>
    {
        public void Configure(EntityTypeBuilder<GroupUser> builder)
        {
            builder.ToTable("GroupUser");

            builder.Property(e => e.GroupCode)
                .HasMaxLength(32)
                .IsUnicode(false);
            builder.Property(e => e.UserCode)
                .HasMaxLength(32)
                .IsUnicode(false);

            builder.HasOne(d => d.Group).WithMany(p => p.GroupUsers)
                .HasForeignKey(d => d.GroupCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupUser_Group");

            builder.HasOne(d => d.User).WithMany(p => p.GroupUsers)
                .HasForeignKey(d => d.UserCode)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_GroupUser_User");
        }
    }
}
