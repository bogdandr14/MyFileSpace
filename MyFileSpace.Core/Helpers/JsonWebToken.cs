using Microsoft.IdentityModel.Tokens;
using MyFileSpace.Infrastructure.Persistence.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFileSpace.Core.Helpers
{
    public static class JsonWebToken
    {
        private const string PASSPHRASE = "ScdfRrGmK*&$riHDciDF*UnvoqNh3GB2Lx3p8tRLc8kkmyaHW!ATUdf4dC7Ubh%q";
        private const string ISSUER_AND_AUDIENCE = "MyFileSpace";
        public const string USER_ROLE_CLAIM = "user_role";

        public static string GenerateToken(User user)
        {
            List<Claim> authClaims = SetClaims(user);

            var authSignIngKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(PASSPHRASE));

            var token = new JwtSecurityToken(
                ISSUER_AND_AUDIENCE,
                ISSUER_AND_AUDIENCE,
                authClaims,
                expires: DateTime.UtcNow.AddMinutes(104),
                signingCredentials: new SigningCredentials(authSignIngKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GetClaim(string token, string claimName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            return securityToken.Claims.FirstOrDefault(claim => claim.Type == claimName)?.Value ?? String.Empty;
        }

        public static bool ValidateToken(string token)
        {
            // Token handler used in order to validate the token
            var tokenHandler = new JwtSecurityTokenHandler();
            // Get the secret key from the jwtSettings instance
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(PASSPHRASE));

            try
            {
                // Validate the token and store it in the validatedToken variable
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    IssuerSigningKey = key,
                    ValidIssuer = ISSUER_AND_AUDIENCE,
                    ValidAudience = ISSUER_AND_AUDIENCE,
                    ClockSkew = TimeSpan.Zero // This is used so that the token expires exactly at its expiry time, not 5 minutes later
                }, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                // Return false if validation fails
                return false;
            }
        }

        private static List<Claim> SetClaims(User user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.TagName),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                new Claim(USER_ROLE_CLAIM, user.Role.ToString())
            };

            return authClaims;
        }
    }
}
