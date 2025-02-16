using MassTransit;
using MassTransit.Courier.Contracts;
using System.Net;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Subscriptions.Services.Poco;
using UbikLink.Security.Contracts.Authorizations.Commands;
using UbikLink.Security.Contracts.Subscriptions.Commands;
using UbikLink.Security.Contracts.Tenants.Commands;
using UbikLink.Security.Contracts.Tenants.Results;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace UbikLink.Security.Api.Mappers
{
    public static class TenantMappers
    {
        public static TenantModel MapToTenant(this AddTenantCommand command)
        {
            return new TenantModel
            {
                Id = NewId.NextGuid(),
                Label = command.Label,
                SubscriptionId = command.SubscriptionId,
                Version = NewId.NextGuid()
            };
        }

        public static TenantModel MapToTenant(this UpdateTenantCommand command, Guid currentId)
        {
            return new TenantModel
            {
                Id = currentId,
                Label = command.Label,
                SubscriptionId = command.SubscriptionId,
                IsActivated = command.IsActivated,
                Version = command.Version
            };
        }

        public static TenantModel MapToTenant(this TenantModel forUpd, TenantModel model)
        {
            model.Id = forUpd.Id;
            model.Label = forUpd.Label;
            model.SubscriptionId = forUpd.SubscriptionId;
            model.IsActivated = forUpd.IsActivated;
            model.Version = forUpd.Version;

            return model;
        }

        public static TenantModel MapToTenant(this UpdateSubscriptionLinkedTenantCommand command, Guid currentId)
        {
            return new TenantModel
            {
                Id = currentId,
                Label = command.Label,
                SubscriptionId = command.SubscriptionId,
                IsActivated = command.Active,
                Version = command.Version
            };
        }

        public static TenantModel MapToTenant(this AddSubscriptionLinkedTenantCommand command)
        {
            return new TenantModel
            {
                Id = NewId.NextGuid(),
                Label = command.Label,
                SubscriptionId = command.SubscriptionId,
                IsActivated = command.Active
            };
        }

        public static TenantStandardResult MapToTenantStandardResult(this TenantModel tenant)
        {
            return new TenantStandardResult
            {
                Id = tenant.Id,
                Label = tenant.Label,
                IsActivated = tenant.IsActivated,
                SubscriptionId = tenant.SubscriptionId,
                Version = tenant.Version,
            };
        }

        public static TenantSubOwnerResult MapToTenantSubOwnerResult(this TenantWithLinkedUsers tenant)
        {
            return new TenantSubOwnerResult
            {
                Id = tenant.Id,
                Label = tenant.Label,
                IsActivated = tenant.IsActivated,
                SubscriptionId = tenant.SubscriptionId,
                Version = tenant.Version,
                LinkedUserIds = tenant.LinkedUsers
            };
        }

        public static IEnumerable<TenantSubOwnerResult> MapToTenantSubOwnerResults(this IEnumerable<TenantWithLinkedUsers> tenants)
        {
            return tenants.Select(tenant => tenant.MapToTenantSubOwnerResult());
        }

        public static IEnumerable<TenantStandardResult> MapToTenantStandardResults(this IEnumerable<TenantModel> tenants)
        {
            return tenants.Select(tenant => tenant.MapToTenantStandardResult());
        }
    }
}
