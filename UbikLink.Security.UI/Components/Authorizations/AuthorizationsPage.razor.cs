using UbikLink.Common.Frontend.Auth;
using UbikLink.Common.RazorUI.Components;
using UbikLink.Security.UI.Shared.Httpclients;

namespace UbikLink.Security.UI.Components.Authorizations
{
    public partial class AuthorizationsPage(ClientUserService clientUserService, 
        IHttpSecurityClient securityClient) : Basepage(clientUserService,securityClient)
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
            }];
    }
}
