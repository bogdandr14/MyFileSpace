using MyFileSpace.Core.DTOs;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Services
{
    public interface IUserAccessService
    {
        Task AddUserAccess(UserAccessUpdateDTO userAccess);
        Task RemoveUserAccess(UserAccessUpdateDTO userAccess);
        Task<List<UserPublicInfoDTO>> GetAllowedUsers(Guid objectId, ObjectType objectType);
    }
}
