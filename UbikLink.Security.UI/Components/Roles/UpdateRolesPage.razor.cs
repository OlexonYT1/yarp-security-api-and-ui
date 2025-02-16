using Microsoft.AspNetCore.Components;
using UbikLink.Common.Frontend.Auth;
using UbikLink.Common.RazorUI.Components;
using UbikLink.Security.UI.Shared.Httpclients;

namespace UbikLink.Security.UI.Components.Roles
{
    public partial class UpdateRolesPage(ClientUserService userService, IHttpSecurityClient securityClient)
        : Basepage(userService, securityClient)
    {
        protected override List<BreadcrumbListItem> BreadcrumbItems
        {
            get
            {
                return _breadcrumbItems;
            }
        }

        [Parameter]
        public Guid Id { get; set; }


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
