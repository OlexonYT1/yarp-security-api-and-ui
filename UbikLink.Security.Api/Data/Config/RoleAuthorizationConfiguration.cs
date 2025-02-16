using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using UbikLink.Security.Api.Data.Models;

namespace UbikLink.Security.Api.Data.Config
{
    public class RoleAuthorizationConfiguration : IEntityTypeConfiguration<RoleAuthorizationModel>
    {
        public void Configure(EntityTypeBuilder<RoleAuthorizationModel> builder)
        {
            builder.ToTable("roles_authorizations");

            builder.HasIndex(a => new { a.RoleId, a.AuthorizationId })
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
                .HasOne<RoleModel>()
                .WithMany()
                .HasForeignKey(u => u.RoleId).OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder
                .HasOne<AuthorizationModel>()
                .WithMany()
                .HasForeignKey(s => s.AuthorizationId).OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
