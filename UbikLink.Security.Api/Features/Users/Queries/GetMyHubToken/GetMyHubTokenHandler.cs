using LanguageExt;
using UbikLink.Common.Api;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Features.Users.Services.Poco;
using UbikLink.Security.Api.Features.Users.Services;
using Microsoft.Extensions.Options;
using UbikLink.Security.Api.Features.Users.Errors;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace UbikLink.Security.Api.Features.Users.Queries.GetMyHubToken
{
    public class GetMyHubTokenHandler(UserQueryService query, ICurrentUser currentUser, IOptions<AuthRegisterAuthKey> keys)
    {
        private readonly UserQueryService _query = query;
        private readonly ICurrentUser _currentUser = currentUser;
        private readonly AuthRegisterAuthKey _keys = keys.Value;

        public async Task<Either<IFeatureError, string>> Handle()
        {
            var user = await _query.GetUserById(_currentUser.Id);

            var connected = user.Match(user =>
            {
                if (user.SelectedTenantId == null)
                    return null;

                var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.UserData, user.SelectedTenantId.ToString()!)};

                var keyBytes = Convert.FromBase64String(_keys.HubSignSecureKey);
                var key = new SymmetricSecurityKey(keyBytes);
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken("UbikLinkHub", "UbikLink", claims, expires: DateTime.UtcNow.AddSeconds(30), signingCredentials: credentials);

                var tokenHandler = new JwtSecurityTokenHandler();
                return tokenHandler.WriteToken(token);

            }, err => null);

            return connected == null ?
                 new GetHubTokenError()
                : connected;
        }
    }
}
