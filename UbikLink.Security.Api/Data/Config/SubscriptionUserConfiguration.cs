using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using UbikLink.Security.Api.Data.Models;

namespace UbikLink.Security.Api.Data.Config
{
    public class SubscriptionUserConfiguration : IEntityTypeConfiguration<SubscriptionUserModel>
    {
        public void Configure(EntityTypeBuilder<SubscriptionUserModel> builder)
        {
            builder.ToTable("subscriptions_users");

            builder.HasIndex(a => new { a.UserId, a.SubscriptionId })
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
                .HasOne<SubscriptionModel>()
                .WithMany()
                .HasForeignKey(s => s.SubscriptionId).OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
