using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Security.Contracts.Authorizations.Results
{
    public record AuthorizationStandardResult
    {
        public Guid Id { get; init; }
        public required string Code { get; init; }
        public string? Description { get; init; }
        public Guid Version { get; init; }
    }
}
