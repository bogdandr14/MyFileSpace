using Ardalis.Result;
using MyFileSpace.Core.Helpers;
using MyFileSpace.SharedKernel.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;

namespace MyFileSpace.Core.Services.Implementation
{
    internal class JwtAuthorizationService : IAuthService
    {
        private readonly Regex _authorizationRegex;
        public JwtAuthorizationService()
        {
            _authorizationRegex = new Regex("Bearer ?<token>[^,]+");
        }

        public async Task<Result<Guid>> IsAuthenticationAndAuthorizationValidAsync(string authorizationString, IEnumerable<RoleType> allowedRoles)
        {
            var token = _authorizationRegex.Match(authorizationString).Groups["token"];
            if (token.Captures.Count != 1)
            {
                return Result.NotFound("Authorization does not contain a JWT token");
            }

            try
            {
                JsonWebToken.ValidateToken(token.Value);
            }
            catch (Exception ex)
            {
                // Return error if validation fails
                List<ValidationError> validationErrors = new List<ValidationError> { };
                do
                {
                    validationErrors.Add(new ValidationError { ErrorMessage = ex.Message, Identifier = ex.Source });
                    ex = ex.InnerException!;
                } while (ex != null);

                return Result.Invalid(validationErrors);
            }

            string userIdString = JsonWebToken.GetClaim(token.Value, JwtRegisteredClaimNames.Jti)!;
            if (string.IsNullOrEmpty(userIdString) || Guid.TryParse(userIdString, out Guid userId))
            {
                return Result.NotFound("The token provided does not containt a valid user id");
            }

            string roleString = JsonWebToken.GetClaim(token.Value, JsonWebToken.USER_ROLE_CLAIM);
            if (string.IsNullOrEmpty(userIdString) || Enum.TryParse(roleString, out RoleType userRole) || !allowedRoles.Contains(userRole))
            {
                return Result.NotFound("User does not have permission to access this resource");
            }

            return Result.Success(userId);
        }
    }
}
