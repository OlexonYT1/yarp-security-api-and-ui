using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UbikLink.Common.Api;

namespace UbikLink.Common.Messaging
{
    public class TenantAndUserIdsConsumeFilter<T>(ICurrentUser currentUser) :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var tenantId = context.Headers.Get<string>("TenantId");
            var userId = context.Headers.Get<string>("UserId");

            currentUser.TenantId = Guid.TryParse(tenantId, out var setTenantId)
                ? setTenantId
                : default;

            currentUser.Id = Guid.TryParse(userId, out var setUserId)
                ? setUserId
                : default;

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
