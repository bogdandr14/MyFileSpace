using Microsoft.AspNetCore.Mvc;
using MyFileSpace.Api.Attributes;
using MyFileSpace.Api.Filters;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Services;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [MyFileSpaceAuthorize]
    [CustomExceptionFilter]
    public class AccessController : ControllerBase
    {
        private readonly IAccessKeyService _accessKeyService;
        private readonly IUserAccessService _userAccessService;

        public AccessController(IAccessKeyService accessKeyService, IUserAccessService userAccessService)
        {
            _accessKeyService = accessKeyService;
            _userAccessService = userAccessService;
        }

        [HttpGet("key/{objectType}/{objectId:Guid}")]
        public async Task<KeyAccessDetailsDTO> GetAccessKey(ObjectType objectType, Guid objectId)
        {
            return await _accessKeyService.GetAccessKey(objectType, objectId);
        }

        [HttpPost("key")]
        public async Task<string> CreateAccessKey(KeyAccesUpdateDTO accessKeyDTO)
        {
            return await _accessKeyService.CreateAccessKey(accessKeyDTO);
        }

        [HttpDelete("key/{objectType}/{objectId:Guid}")]
        public async Task DeleteAccessKey(ObjectType objectType, Guid objectId)
        {
            await _accessKeyService.DeleteAccessKey(objectType, objectId);
        }

        [HttpGet("user/{objectType}/{objectId:Guid}")]
        public async Task<List<UserPublicInfoDTO>> GetUserAccess(ObjectType objectType, Guid objectId)
        {
            return await _userAccessService.GetAllowedUsers(objectId, objectType);
        }

        [HttpPost("user")]
        public async Task AddUserAccess(UserAccessUpdateDTO userAccessDTO)
        {
            await _userAccessService.AddUserAccess(userAccessDTO);
        }

        [HttpDelete("user")]
        public async Task RemoveUserAccess(UserAccessUpdateDTO userAccessDTO)
        {
            await _userAccessService.RemoveUserAccess(userAccessDTO);
        }
    }
}
