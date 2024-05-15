using Microsoft.AspNetCore.Mvc;
using MyFileSpace.Api.Attributes;
using MyFileSpace.Caching;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Services;
using MyFileSpace.SharedKernel;
using MyFileSpace.SharedKernel.Enums;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyFileSpace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly ICacheManager _cacheManager;
        private readonly IStoredFileService _storedFileService;

        public ManagementController(ICacheManager cacheManager, IStoredFileService storedFileService)
        {
            _cacheManager = cacheManager;
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

        public async Task<MemorySize> GetAllowedStorage()
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

        public async Task<MemorySize> GetMemoryUsage()
        {
            return await _cacheManager.GetMemoryUsedAsync();
        }

        // DELETE api/<CacheController>
        [HttpDelete("cacheClear")]
        [MyFileSpaceAuthorize(RoleType.Admin)]

        public async Task Delete()
        {
            await _cacheManager.ClearAsync();
        }
    }
}
