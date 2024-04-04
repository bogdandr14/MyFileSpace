using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Services
{
    public interface IAuthService
    {
        Tuple<Guid, RoleType> ValidateUserAuthorization(string authorizationString, IEnumerable<RoleType> allowedRoles);
    }
}
