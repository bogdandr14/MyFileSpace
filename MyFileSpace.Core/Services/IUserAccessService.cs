using MyFileSpace.Core.DTOs;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Services
{
    public interface IUserAccessService
    {
        Task EditAllowedUsers(UserAccessUpdateDTO userAccess);
        Task<List<UserDTO>> GetAllowedUsers(Guid objectId, ObjectType objectType);
    }
}
