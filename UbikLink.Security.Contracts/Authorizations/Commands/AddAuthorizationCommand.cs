using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Security.Contracts.Authorizations.Commands
{
    public record AddAuthorizationCommand
    {
        public required string Code { get; init; }
        public string? Description { get; init; }
    }
}
