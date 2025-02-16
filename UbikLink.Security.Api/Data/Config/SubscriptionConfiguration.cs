using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UbikLink.Security.Api.Data.Models;

namespace UbikLink.Security.Api.Data.Config
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<SubscriptionModel>
    {
        public void Configure(EntityTypeBuilder<SubscriptionModel> builder)
        {
            builder.Property(a => a.Version)
                .IsConcurrencyToken();
        }
    }
}
