using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyFileSpace.Api.Providers;
using MyFileSpace.Core.Services;
using MyFileSpace.SharedKernel.Enums;
using MyFileSpace.SharedKernel.Helpers;

namespace MyFileSpace.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MyFileSpaceAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IEnumerable<RoleType> rolesAllowed;
        private bool _providersInitialized = false;
        private IHttpContextProvider _httpContextProvider;
        private IAuthService _authService;
        private string _authorizationString;

        public MyFileSpaceAuthorizeAttribute()
        {
            rolesAllowed = new List<RoleType>
            {
                RoleType.Admin,
                RoleType.Customer
            };
        }

        public MyFileSpaceAuthorizeAttribute(RoleType role)
        {
            rolesAllowed = new List<RoleType>
            {
                role
            };
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                InitializeProviders(context.HttpContext.RequestServices);

                // validate required headers
                if (!ValidateHeaders(ref context))
                {
                    return;
                }

                // validate user authentication
                if (!IsAuthenticationAndAuthorizationValid(ref context))
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                context.Result = GetActionResult(ex, StatusCodes.Status401Unauthorized);
            }
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

        private bool ValidateHeaders(ref AuthorizationFilterContext context)
        {
            // Retrieve required headers
            _authorizationString = _httpContextProvider.GetValueFromRequestHeader(Constants.AUTH_HEADER);
            if (_authorizationString == null)
            {
                context.Result = GetActionResult($"The ${Constants.AUTH_HEADER} is missing", StatusCodes.Status401Unauthorized);
                return false;
            }

            return true;
        }

        private bool IsAuthenticationAndAuthorizationValid(ref AuthorizationFilterContext context)
        {
            Result<Guid> authenticatedResult = _authService.IsAuthenticationAndAuthorizationValidAsync(_authorizationString, rolesAllowed).GetAwaiter().GetResult();

            if (!authenticatedResult.IsSuccess)
            {
                context.Result = GetActionResult(authenticatedResult);
                return false;
            }

            return true;
        }

        private static IActionResult GetActionResult<T>(Result<T> serviceResult)
        {
            switch (serviceResult.Status)
            {
                case ResultStatus.Forbidden:
                    return GetActionResult("The user is not allowed to access the specific resource", StatusCodes.Status403Forbidden);
                case ResultStatus.NotFound:
                    string? message = serviceResult.Errors != null ? $"The following information is missing: {serviceResult.Errors.ToString<string>()}" : serviceResult.Status.ToString();
                    return GetActionResult(message, StatusCodes.Status404NotFound);
                case ResultStatus.Error:
                    return GetActionResult($"Encountered the following errors: {serviceResult.Errors.ToString<string>()}", StatusCodes.Status500InternalServerError);
                case ResultStatus.Invalid:
                    return GetActionResult($"The following validations did not pass:{serviceResult.ValidationErrors.ToString<ValidationError>()}", StatusCodes.Status500InternalServerError);
                default:
                    return GetActionResult(serviceResult.Errors.ToString()!, StatusCodes.Status500InternalServerError);
            }
        }

        private static IActionResult GetActionResult(object? value, int statusCode)
        {
            return new JsonResult(value) { StatusCode = statusCode };
        }
    }
}
