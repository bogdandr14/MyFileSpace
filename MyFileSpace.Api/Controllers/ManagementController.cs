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
    [MyFileSpaceAuthorize(RoleType.Admin)]
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
        public async Task<FileStatisticsDTO> GetStatistics()
        {
            return await _storedFileService.GetStatistics();
        }

        [HttpDelete("pastRetention")]
        public async Task PermanentDelete()
        {
            await _storedFileService.DeletePastBinRetention();
        }

        // PUT api/<CacheController>/usage
        [HttpGet("cacheUsage")]
        public async Task<string> GetMemoryUsage()
        {
            return $"{await _cacheService.GetMemoryMbUsed()}Mb";
        }

        // DELETE api/<CacheController>
        [HttpDelete("cacheClear")]
        public async Task Delete()
        {
            await _cacheService.ClearCache();
        }
    }
}
