using LanguageExt;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Tenants.Services;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Tenants.Commands;


namespace UbikLink.Security.Api.Features.Tenants.Commands.AddTenantForAdmin
{
    public class AddTenantForAdminHandler(TenantCommandService commandService)
    {
        private readonly TenantCommandService _commandService = commandService;

        public async Task<Either<IFeatureError, TenantModel>> Handle(AddTenantCommand command)
        {
            var tenant = command.MapToTenant();

            return await _commandService.ValidateIfSubscriptionExistsAsync(tenant)
                    .BindAsync(ts => _commandService.ValidateTenantLimitForSubscriptionAsync(ts.Item1, ts.Item2,false))
                    .BindAsync(_commandService.AddTenantInDbAsync);
        }
    }
}
