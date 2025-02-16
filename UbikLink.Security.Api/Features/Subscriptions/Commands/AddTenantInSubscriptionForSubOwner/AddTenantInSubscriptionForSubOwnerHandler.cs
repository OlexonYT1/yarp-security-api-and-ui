using LanguageExt;
using UbikLink.Common.Api;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Subscriptions.Services;
using UbikLink.Security.Api.Features.Subscriptions.Services.Poco;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Authorizations.Commands;
using UbikLink.Security.Contracts.Subscriptions.Commands;

namespace UbikLink.Security.Api.Features.Subscriptions.Commands.AddTenantInSubscriptionForSubOwner
{
    public class AddTenantInSubscriptionForSubOwnerHandler(SubscriptionCommandService commandService, ICurrentUser currentUser)
    {
        private readonly SubscriptionCommandService _commandService = commandService;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<Either<IFeatureError, TenantWithLinkedUsers>> Handle(AddSubscriptionLinkedTenantCommand command)
        {
            var current = command.MapToTenant();

            return await _commandService.GetSelectedSubscriptionForOwnerAsync(_currentUser.Id)
                .BindAsync(s => _commandService.ValidateIfTenantSubscriptionIsSameAsSelectedForOwnerAsync(current, s))
                .BindAsync(ts => _commandService.ValidateTenantLimitForSubscriptionAsync(ts.Tenant,ts.Subscription,false))
                .BindAsync(t=> _commandService.PrepareUserIdsForAttachAndDetachFromTenantAsync(t, command.LinkedUsersIds,false))
                .BindAsync(tu => _commandService.AddTenantWithLinkedUsersInDbAsync(tu.Tenant, tu.UserIdsForInsert));
        }
    }
}
