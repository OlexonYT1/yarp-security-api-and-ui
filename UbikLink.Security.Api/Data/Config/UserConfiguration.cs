using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using UbikLink.Security.Api.Data.Models;

namespace UbikLink.Security.Api.Data.Config
{
    public class UserConfiguration : IEntityTypeConfiguration<UserModel>
    {
        public void Configure(EntityTypeBuilder<UserModel> builder)
        {
            builder.Property(a => a.AuthId)
                .HasMaxLength(100);

            builder.Property(a => a.Firstname)
                .HasMaxLength(100);

            builder.Property(a => a.Lastname)
                .HasMaxLength(100);

            builder.Property(a => a.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.ActivationCode)
                .HasMaxLength(50);

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

            builder.HasIndex(a => a.Email)
                .IsUnique(false);

            builder.HasIndex(a => a.AuthId)
                .IsUnique();

            builder
                .HasOne<TenantModel>()
                .WithMany()
                .HasForeignKey(e => e.SelectedTenantId).OnDelete(DeleteBehavior.SetNull).IsRequired(false);
        }
    }
}
