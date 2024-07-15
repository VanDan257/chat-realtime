using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using notip_server.Models;
using System.Reflection.Emit;

namespace notip_server.Data.Configurations
{
    public class CallConfiguration : IEntityTypeConfiguration<Call>
    {
        public void Configure(EntityTypeBuilder<Call> builder)
        {

            builder.ToTable("Call");

            builder.Property(e => e.Created).HasColumnType("datetime");
            builder.Property(e => e.GroupCallCode)
                .HasMaxLength(32)
                .IsUnicode(false);
            builder.Property(e => e.Status)
                .HasMaxLength(32)
                .IsUnicode(false);
            builder.Property(e => e.Url).HasMaxLength(500);
            builder.Property(e => e.UserCode)
                .HasMaxLength(32)
                .IsUnicode(false);

            builder.HasOne(d => d.GroupCall).WithMany(p => p.Calls)
                .HasForeignKey(d => d.GroupCallCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Call_GroupCall");

            builder.HasOne(d => d.User).WithMany(p => p.Calls)
                .HasForeignKey(d => d.UserCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Call_User");
        }
    }
}
