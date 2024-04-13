using MyFileSpace.Core.Helpers;
using MyFileSpace.SharedKernel.Enums;
using MyFileSpace.SharedKernel.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;

namespace MyFileSpace.Core.Services.Implementation
{
    internal class JwtAuthorizationService : IAuthService
    {
        private readonly Regex _authorizationRegex;
        public JwtAuthorizationService()
        {
            _authorizationRegex = new Regex("Bearer (?<token>[^,]+)");
        }

        public Tuple<Guid, RoleType> ValidateUserAuthorization(string authorizationString, IEnumerable<RoleType> allowedRoles)
        {
            Group? tokenGroup = _authorizationRegex.Match(authorizationString).Groups["token"];
            if (tokenGroup == null || tokenGroup.Captures.Count != 1)
            {
                throw new UnauthorizedException("Missing the JWT token for authorization");
            }

            try
            {
                JsonWebToken.ValidateToken(tokenGroup.Value);
            }
            catch (Exception ex)
            {
                throw new UnauthorizedException("Token validation failed", ex);
            }

            string userIdString = JsonWebToken.GetClaim(tokenGroup.Value, JwtRegisteredClaimNames.Jti)!;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                throw new NotFoundException("The token provided does not containt a valid user id");
            }

            string roleString = JsonWebToken.GetClaim(tokenGroup.Value, JsonWebToken.USER_ROLE_CLAIM);
            if (string.IsNullOrEmpty(userIdString) || !Enum.TryParse(roleString, out RoleType userRole) || !allowedRoles.Contains(userRole))
            {
                throw new ForbiddenException("User does not have permission to access this resource");
            }

            return Tuple.Create(userId, userRole);
        }
    }
}
