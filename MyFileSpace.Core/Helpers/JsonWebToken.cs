using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.SharedKernel.Providers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFileSpace.Core.Helpers
{
    internal static class JsonWebToken
    {
        private static string _passphrase;
        private static string _issuer;
        private static string _audience;

        private static string GetPassphrase()
        {
            if (string.IsNullOrEmpty(_passphrase))
            {
                _passphrase = AppServicesProvider.GetService<IConfiguration>().GetConfigValue("TokenGenerator:Passphrase");
            }
            return _passphrase;
        }

        private static string GetIssuer()
        {
            if (string.IsNullOrEmpty(_issuer))
            {
                _issuer = AppServicesProvider.GetService<IConfiguration>().GetConfigValue("TokenGenerator:Issuer");
            }
            return _issuer;
        }

        private static string GetAudience()
        {
            if (string.IsNullOrEmpty(_audience))
            {
                _audience = AppServicesProvider.GetService<IConfiguration>().GetConfigValue("TokenGenerator:Audience");
            }
            return _audience;
        }

        internal static string GenerateToken(User user)
        {
            List<Claim> authClaims = SetClaims(user);

            var authSignIngKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetPassphrase()));

            var token = new JwtSecurityToken(
                GetIssuer(),
                GetAudience(),
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

        internal static bool ValidateToken(string token)
        {
            // Token handler used in order to validate the token
            var tokenHandler = new JwtSecurityTokenHandler();
            // Get the secret key from the jwtSettings instance
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetPassphrase()));

            // Validate the token and store it in the validatedToken variable
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                IssuerSigningKey = key,
                ValidIssuer = GetIssuer(),
                ValidAudience = GetAudience(),
                ClockSkew = TimeSpan.Zero // This is used so that the token expires exactly at its expiry time, not 5 minutes later
            }, out SecurityToken validatedToken);

            return true;
        }

        private static List<Claim> SetClaims(User user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                new Claim(Constants.USER_ROLE_CLAIM, user.Role.ToString())
            };

            if (!string.IsNullOrEmpty(user.TagName))
            {
                authClaims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, user.TagName));
            }
            return authClaims;
        }
    }
}
