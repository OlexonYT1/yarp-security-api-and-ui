using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Net;
using UbikLink.Security.Api.Data.Models;

namespace UbikLink.Security.Api.Data.Config
{
    public class AuthorizationConfiguration : IEntityTypeConfiguration<AuthorizationModel>
    {
        public void Configure(EntityTypeBuilder<AuthorizationModel> builder)
        {
            builder.Property(a => a.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(a => a.Code)
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
        }
    }
}
