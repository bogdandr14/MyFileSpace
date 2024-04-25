using Microsoft.AspNetCore.Mvc.Filters;
using MyFileSpace.Api.Providers;
using MyFileSpace.Core;
using MyFileSpace.Core.Services;
using MyFileSpace.SharedKernel.Enums;
using MyFileSpace.SharedKernel.Exceptions;

namespace MyFileSpace.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MyFileSpaceAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IEnumerable<RoleType> rolesAllowed;
        private readonly bool allowAnonymous;
        private bool _providersInitialized = false;
        private IHttpContextProvider _httpContextProvider;
        private IAuthService _authService;
        private string _authorizationString;

        public MyFileSpaceAuthorizeAttribute(bool allowAnonymous = false)
        {
            rolesAllowed = new List<RoleType>
            {
                RoleType.Admin,
                RoleType.Customer
            };
            this.allowAnonymous = allowAnonymous;
        }

        public MyFileSpaceAuthorizeAttribute(RoleType role)
        {
            rolesAllowed = new List<RoleType>
            {
                role
            };
            allowAnonymous = false;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            InitializeProviders(context.HttpContext.RequestServices);

            // validate required headers
            _authorizationString = _httpContextProvider.GetValueFromRequestHeader(Constants.AUTH_HEADER);
            if (_authorizationString == null)
            {
                if (allowAnonymous)
                {
                    return;
                }

                throw new UnauthorizedException($"The ${Constants.AUTH_HEADER} is missing");
            }

            // validate user authentication
            (Guid guid, RoleType roleType) = _authService.ValidateUserAuthorization(_authorizationString, rolesAllowed);

            // set session info
            Session session = (Session)context.HttpContext.RequestServices.GetService(typeof(Session))!;
            session.IsAuthenticated = true;
            session.UserId = guid;
            session.Role = roleType;
        }

        private void InitializeProviders(IServiceProvider serviceProvider)
        {
            // already initialized
            if (_providersInitialized)
            {
                return;
            }

            _httpContextProvider = (IHttpContextProvider)serviceProvider.GetService(typeof(IHttpContextProvider))!;
            _authService = (IAuthService)serviceProvider.GetService(typeof(IAuthService))!;
            _providersInitialized = true;
        }
    }
}
