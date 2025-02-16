using LanguageExt;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Tenants.Services;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Tenants.Commands;

namespace UbikLink.Security.Api.Features.Tenants.Commands.UpdateTenantForAdmin
{
    public class UpdateTenantForAdminHandler(TenantCommandService commandService)
    {
        private readonly TenantCommandService _commandService = commandService;

        public async Task<Either<IFeatureError, TenantModel>> Handle(UpdateTenantCommand command, Guid currentId)
        {
            var upd = command.MapToTenant(currentId);

            return await _commandService.GetTenantByIdAsync(upd.Id)
                    .BindAsync(t => _commandService.MapInDbContextAsync(t, upd))
                    .BindAsync(_commandService.ValidateIfSubscriptionExistsAsync)
                    .BindAsync(ts => _commandService.ValidateTenantLimitForSubscriptionAsync(ts.Item1, ts.Item2,true))
                    .BindAsync(_commandService.UpdateTenantInDbAsync);
        }
    }
}
