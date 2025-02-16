using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using UbikLink.Security.Api.Data.Models;

namespace UbikLink.Security.Api.Data.Config
{
    public class TenantUserConfiguration : IEntityTypeConfiguration<TenantUserModel>
    {
        public void Configure(EntityTypeBuilder<TenantUserModel> builder)
        {
            builder.ToTable("tenants_users");

            builder.HasIndex(a => new { a.UserId, a.TenantId })
                 .IsUnique();

            builder.Property(a => a.Version)
                .IsConcurrencyToken();

            builder.OwnsOne(x => x.AuditInfo, auditInfo =>
            {
                auditInfo.Property(a => a.ModifiedAt)
                    .HasColumnName("modified_at")
                    .IsRequired();

                auditInfo.Property(a => a.ModifiedBy)
                    .HasColumnName("modified_by")
                    .IsRequired();

                auditInfo.Property(a => a.CreatedAt)
                    .HasColumnName("created_at")
                    .IsRequired();

                auditInfo.Property(a => a.CreatedBy)
                    .HasColumnName("created_by")
                    .IsRequired();
            });

            builder
                .HasOne<UserModel>()
                .WithMany()
                .HasForeignKey(u => u.UserId).OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder
                .HasOne<TenantModel>()
                .WithMany()
                .HasForeignKey(s => s.TenantId).OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
