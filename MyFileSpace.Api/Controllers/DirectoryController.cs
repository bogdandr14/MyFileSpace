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

        [HttpGet("info/{directoryId:Guid}")]
        [MyFileSpaceAuthorize(true)]
        public async Task<DirectoryDetailsDTO> GetDirectoryInfoById(Guid directoryId, [FromQuery] string? accessKey)
        {
            return await _virtualDirectoryService.GetDirectoryInfo(directoryId, accessKey);
        }

        [HttpPost("{parentDirectoryId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<DirectoryDTO> AddDirectory(DirectoryUpdateDTO directory, Guid parentDirectoryId)
        {
            return await _virtualDirectoryService.AddDirectory(directory, parentDirectoryId);
        }

        [HttpPut("{directoryId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<DirectoryDTO> UpdateDirectory(DirectoryUpdateDTO directory, Guid directoryId)
        {
            return await _virtualDirectoryService.UpdateDirectory(directory, directoryId);
        }

        [HttpPut("move/{directoryId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task MoveDirectory(Guid directoryId, Guid newParentDirectoryId)
        {
            await _virtualDirectoryService.MoveToDirectory(directoryId, newParentDirectoryId);
        }

        [HttpPut("restore/{directoryId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task RestoreDirectory(Guid directoryId)
        {
            await _virtualDirectoryService.RestoreDirectory(directoryId);
        }

        [HttpDelete("{directoryId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task MoveToBin(Guid directoryId)
        {
            await _virtualDirectoryService.MoveDirectoryToBin(directoryId);
        }

        [HttpDelete("permanent/{directoryId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task Delete(Guid directoryId)
        {
            await _virtualDirectoryService.DeleteDirectory(directoryId);
        }
    }
}
