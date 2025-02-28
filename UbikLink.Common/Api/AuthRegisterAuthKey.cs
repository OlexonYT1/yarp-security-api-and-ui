using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Common.Api
{
    //TODO: don't forget to activate the email activation
    public class AuthRegisterAuthKey
    {
        public const string Position = "AuthRegister";
        public string Key { get; set; } = string.Empty;
        public bool EmailActivationActivated { get; set; } = false;
    }
}
