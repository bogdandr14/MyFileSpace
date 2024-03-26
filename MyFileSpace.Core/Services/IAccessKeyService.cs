using MyFileSpace.Core.DTOs;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Services
{
    public interface IAccessKeyService
    {
        Task<string> CreateAccessKey(KeyAccesUpdateDTO keyAccess);
        Task<KeyAccessDetailsDTO> GetAccessKey(ObjectType objectType, Guid objectId);
        Task DeleteAccessKey(ObjectType objectType, Guid objectId);
    }
}
