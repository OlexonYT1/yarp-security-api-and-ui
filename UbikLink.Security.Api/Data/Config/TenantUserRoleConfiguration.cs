using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using UbikLink.Security.Api.Data.Models;

namespace UbikLink.Security.Api.Data.Config
{
    public class TenantUserRoleConfiguration : IEntityTypeConfiguration<TenantUserRoleModel>
    {
        public void Configure(EntityTypeBuilder<TenantUserRoleModel> builder)
        {
            builder.ToTable("tenants_users_roles");

            builder.HasIndex(a => new { a.TenantUserId, a.RoleId })
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
                .HasOne<TenantUserModel>()
                .WithMany()
                .HasForeignKey(u => u.TenantUserId).OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder
                .HasOne<RoleModel>()
                .WithMany()
                .HasForeignKey(s => s.RoleId).OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
