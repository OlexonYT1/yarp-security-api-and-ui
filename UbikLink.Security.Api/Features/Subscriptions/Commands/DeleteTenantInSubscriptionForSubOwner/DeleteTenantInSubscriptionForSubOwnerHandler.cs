using LanguageExt;
using UbikLink.Common.Api;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Features.Subscriptions.Services.Poco;
using UbikLink.Security.Api.Features.Subscriptions.Services;
using UbikLink.Security.Contracts.Subscriptions.Commands;

namespace UbikLink.Security.Api.Features.Subscriptions.Commands.DeleteTenantInSubscriptionForSubOwner
{
    public class DeleteTenantInSubscriptionForSubOwnerHandler(SubscriptionCommandService commandService, ICurrentUser currentUser)
    {
        private readonly SubscriptionCommandService _commandService = commandService;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<Either<IFeatureError, Guid>> Handle(Guid currentId)
        {
            return await _commandService.GetSelectedSubscriptionForOwnerAsync(_currentUser.Id)
                .BindAsync(s => _commandService.GetTenantInSubscriptionAsyc(currentId,s))
                .BindAsync(tu => _commandService.DeleteTenantInDbAsync(tu.Tenant));
        }
    }
}
