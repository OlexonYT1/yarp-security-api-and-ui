using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Security.Contracts.Authorizations.Commands
{
    public record UpdateAuthorizationCommand
    {
        public required string Code { get; init; }
        public string? Description { get; init; }
        public required Guid Version { get; init; }
    }
}
