using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Security.Contracts.Users.Commands
{
    public class SetSettingsUserMeCommand
    {
        public Guid TenantId { get; init; }
    }
}
