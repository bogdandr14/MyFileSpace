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

        [HttpGet("{directoryId:Guid}")]
        [MyFileSpaceAuthorize(true)]
        public async Task<DirectoryDetailsDTO> GetDirectoryInfoById(Guid directoryId, [FromQuery] string? accessKey)
        {
            return await _virtualDirectoryService.GetDirectoryInfo(directoryId, accessKey);
        }

        [HttpPost]
        [MyFileSpaceAuthorize]
        public async Task<DirectoryDTO> AddDirectory(DirectoryCreateDTO directory)
        {
            return await _virtualDirectoryService.AddDirectory(directory);
        }

        [HttpPut]
        [MyFileSpaceAuthorize]
        public async Task<DirectoryDTO> UpdateDirectory(DirectoryUpdateDTO directory)
        {
            return await _virtualDirectoryService.UpdateDirectory(directory);
        }

        [HttpPut("move/{directoryId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task MoveDirectory(Guid directoryId, [FromQuery]Guid newParentDirectoryId, [FromQuery] bool restore = false)
        {
            await _virtualDirectoryService.MoveDirectory(directoryId, newParentDirectoryId, restore);
        }

        [HttpDelete("{directoryId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task Delete(Guid directoryId, [FromQuery] bool permanent = false)
        {
            await _virtualDirectoryService.DeleteDirectory(directoryId, permanent);
        }
    }
}
