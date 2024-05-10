using Microsoft.AspNetCore.Mvc;
using MyFileSpace.Api.Attributes;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Services;
using MyFileSpace.SharedKernel.Enums;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyFileSpace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly IStoredFileService _storedFileService;

        public ManagementController(ICacheService cacheService, IStoredFileService storedFileService)
        {
            _cacheService = cacheService;
            _storedFileService = storedFileService;
        }

        [HttpGet("statistics")]
        [MyFileSpaceAuthorize(RoleType.Admin)]

        public async Task<FileStatisticsDTO> GetStatistics()
        {
            return await _storedFileService.GetStatistics();
        }
        [HttpGet("allowedStorage")]
        [MyFileSpaceAuthorize]

        public async Task<MemorySizeDTO> GetAllowedStorage()
        {
            return await _storedFileService.GetAllowedStorage();
        }

        [HttpDelete("pastRetention")]
        [MyFileSpaceAuthorize(RoleType.Admin)]

        public async Task PermanentDelete()
        {
            await _storedFileService.DeletePastBinRetention();
        }

        // PUT api/<CacheController>/usage
        [HttpGet("cacheUsage")]
        [MyFileSpaceAuthorize(RoleType.Admin)]

        public async Task<MemorySizeDTO> GetMemoryUsage()
        {
            return await _cacheService.GetMemoryUsed();
        }

        // DELETE api/<CacheController>
        [HttpDelete("cacheClear")]
        [MyFileSpaceAuthorize(RoleType.Admin)]

        public async Task Delete()
        {
            await _cacheService.ClearCache();
        }
    }
}
