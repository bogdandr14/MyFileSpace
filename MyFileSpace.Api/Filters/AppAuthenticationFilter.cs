using Microsoft.AspNetCore.Mvc.Filters;
using MyFileSpace.Core;
using MyFileSpace.Core.Helpers;
using MyFileSpace.SharedKernel.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyFileSpace.Api.Filters
{
    public class AppAuthenticationFilter : IAsyncActionFilter
    {
        private Session _session;

        public AppAuthenticationFilter(Session session)
        {
            _session = session;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.User.Identity != null)
            {
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)context.HttpContext.User.Identity;
                Claim? userIdClaim = claimsIdentity.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
                
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userGuid))
                {
                    _session.IsAuthenticated = true;
                    _session.UserId = userGuid;
                }
                
                Claim? roleClaim = claimsIdentity.Claims.SingleOrDefault(c => c.Type == JsonWebToken.USER_ROLE_CLAIM);
                if(roleClaim != null && Enum.TryParse(roleClaim.Value, out RoleType roleType))
                {
                    _session.Role = roleType;
                }
            }

            var resultContext = await next();
        }
    }
}
