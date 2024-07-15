using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using notip_server.Models;

namespace notip_server.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("User");

            builder.Property(e => e.Id)
                .HasMaxLength(32)
                .IsUnicode(false);
            builder.Property(e => e.Address).HasMaxLength(255);
            builder.Property(e => e.Avatar).IsUnicode(false);
            builder.Property(e => e.Dob)
                .HasMaxLength(50)
                .IsUnicode(false);
            builder.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            builder.Property(e => e.UserName).HasMaxLength(50);
            builder.Property(e => e.Gender).HasMaxLength(10);
            builder.Property(e => e.LastLogin).HasColumnType("datetime");
            builder.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            builder.Property(e => e.PasswordSalt)
                .HasMaxLength(255)
                .IsUnicode(false);
            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
        }
    }
}
