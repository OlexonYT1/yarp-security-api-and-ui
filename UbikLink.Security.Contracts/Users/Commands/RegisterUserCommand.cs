using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Security.Contracts.Users.Commands
{
    public record RegisterUserCommand
    {
        public required string AuthId { get; init; }
        public required string Firstname { get; init; }
        public required string Lastname { get; init; }
        public required string Email { get; init; }
        public required string AuthorizationKey { get; init; }
    }
}
