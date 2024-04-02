using Microsoft.AspNetCore.Mvc;
using MyFileSpace.Api.Attributes;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Services;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [MyFileSpaceAuthorize]

    public class AccessController : ControllerBase
    {
        private readonly IAccessKeyService _accessKeyService;
        private readonly IUserAccessService _userAccessService;

        public AccessController(IAccessKeyService accessKeyService, IUserAccessService userAccessService)
        {
            _accessKeyService = accessKeyService;
            _userAccessService = userAccessService;
        }

        [HttpGet("key/{objectType:ObjectType}/{objectId:Guid}")]
        public async Task<ActionResult<KeyAccessDetailsDTO>> GetAccessKey(ObjectType objectType, Guid objectId)
        {
            try
            {
                return Ok(await _accessKeyService.GetAccessKey(objectType, objectId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("key")]
        public async Task<ActionResult<string>> CreateAccessKey(KeyAccesUpdateDTO accessKeyDTO)
        {
            try
            {
                return Ok(await _accessKeyService.CreateAccessKey(accessKeyDTO));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("key/{objectType:ObjectType}/{objectId:Guid}")]
        public async Task<ActionResult> DeleteAccessKey(ObjectType objectType, Guid objectId)
        {
            try
            {
                await _accessKeyService.DeleteAccessKey(objectType, objectId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("user/{objectType:ObjectType}/{objectId:Guid}")]
        public async Task<ActionResult<List<UserPublicInfoDTO>>> GetUserAccess(ObjectType objectType, Guid objectId)
        {
            try
            {
                return Ok(await _userAccessService.GetAllowedUsers(objectId, objectType));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("user")]
        public async Task<ActionResult> AddUserAccess(UserAccessUpdateDTO userAccessDTO)
        {
            try
            {
                await _userAccessService.AddUserAccess(userAccessDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("user")]
        public async Task<ActionResult> RemoveUserAccess(UserAccessUpdateDTO userAccessDTO)
        {
            try
            {
                await _userAccessService.RemoveUserAccess(userAccessDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
