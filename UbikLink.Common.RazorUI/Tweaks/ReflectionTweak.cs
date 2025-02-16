using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Common.RazorUI.Tweaks
{
    public static class ReflectionTweak
    {
        public static T? GetPrivateFieldValue<T>(object obj, string fieldName)
        {
            return (T?)obj.GetType()
                          .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)?
                          .GetValue(obj);
        }
    }
}
