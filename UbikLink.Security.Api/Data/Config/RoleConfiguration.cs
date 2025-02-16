using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Data;
using UbikLink.Security.Api.Data.Models;

namespace UbikLink.Security.Api.Data.Config
{
    public class RoleConfiguration : IEntityTypeConfiguration<RoleModel>
    {
        public void Configure(EntityTypeBuilder<RoleModel> builder)
        {
            builder.Property(a => a.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(a => new { a.Code, a.TenantId })
                 .IsUnique();

            builder.Property(a => a.Description)
                .HasMaxLength(500);

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

            //When it's a role specific to a tenant
            builder
                .HasOne<TenantModel>()
                .WithMany()
                .HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
        }
    }
}
