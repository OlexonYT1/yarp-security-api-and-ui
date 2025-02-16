using UbikLink.Common.Frontend.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using UbikLink.Common.RazorUI.Components;
using UbikLink.Security.UI.Shared.Httpclients;

namespace UbikLink.Security.UI.Components.Authorizations
{
    public partial class AddAuthorizationPage(ClientUserService userService, IHttpSecurityClient securityClient)
        : Basepage(userService, securityClient)
    {
        protected override List<BreadcrumbListItem> BreadcrumbItems
        {
            get
            {
                return _breadcrumbItems;
            }
        }

        private static readonly List<BreadcrumbListItem> _breadcrumbItems = [
            new BreadcrumbListItem()
                                            {
                                                Position =1,
                                                Url = "/",
                                                Name = "Home"
                                            },
                                            new BreadcrumbListItem()
                                            {
                                                Position = 2,
                                                Url = "/authorizations?state=true",
                                                Name = "Authorizations"
                                            }
            ];
    }
}
