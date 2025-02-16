using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Security.Contracts.Authorizations.Commands
{
    public record BatchDeleteAuthorizationCommand
    {
        public List<Guid> AuthorizationIds { get; init; } = [];
    }
}
