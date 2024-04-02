using Microsoft.AspNetCore.Mvc;
using MyFileSpace.Api.Attributes;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Services;

namespace MyFileSpace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectoryController : ControllerBase
    {
        private readonly IVirtualDirectoryService _virtualDirectoryService;

        public DirectoryController(IVirtualDirectoryService virtualDirectoryService)
        {
            _virtualDirectoryService = virtualDirectoryService;
        }

        [HttpGet]
        [MyFileSpaceAuthorize]
        public async Task<List<DirectoryDTO>> GetDirectories()
        {
            return await _virtualDirectoryService.GetAllDirectoriesInfo();
        }

        [HttpGet("info/{directoryId: Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<ActionResult<DirectoryDetailsDTO>> GetDirectoryInfo(Guid directoryId)
        {
            try
            {
                return Ok(await _virtualDirectoryService.GetDirectoryInfo(directoryId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("info/{directoryId: Guid}/{accessKey}")]
        public async Task<ActionResult<DirectoryDetailsDTO>> GetDirectoryInfo(Guid directoryId, string accessKey)
        {
            try
            {

                return Ok(await _virtualDirectoryService.GetDirectoryInfo(directoryId, accessKey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("{parentDirectoryId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<ActionResult> AddDirectory(DirectoryUpdateDTO directory, Guid parentDirectoryId)
        {
            try
            {
                await _virtualDirectoryService.AddDirectory(directory, parentDirectoryId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("{directoryId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<ActionResult> UpdateDirectory(DirectoryUpdateDTO directory, Guid directoryId)
        {
            try
            {
                await _virtualDirectoryService.UpdateDirectory(directory, directoryId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("move/{directoryId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<ActionResult> MoveDirectory(Guid directoryId, Guid newParentDirectoryId)
        {
            try
            {
                await _virtualDirectoryService.MoveToDirectory(directoryId, newParentDirectoryId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("restore/{directoryId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<ActionResult> RestoreDirectory(Guid directoryId)
        {
            try
            {
                await _virtualDirectoryService.RestoreDirectory(directoryId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("{directoryId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<ActionResult> MoveToBin(Guid directoryId)
        {
            try
            {
                await _virtualDirectoryService.MoveDirectoryToBin(directoryId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpDelete("permanent/{directoryId:Guid}")]
        [MyFileSpaceAuthorize]

        public async Task<ActionResult> Delete(Guid directoryId)
        {
            try
            {
                await _virtualDirectoryService.DeleteDirectory(directoryId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
