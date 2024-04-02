using Ardalis.Result;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Services
{
    public interface IAuthService
    {
        Task<Result<Guid>> IsAuthenticationAndAuthorizationValidAsync(string authorizationString, IEnumerable<RoleType> allowedRoles);
    }
}
