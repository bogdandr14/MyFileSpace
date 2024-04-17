using Microsoft.AspNetCore.Mvc;
using MyFileSpace.Api.Attributes;
using MyFileSpace.Core.Services;
using MyFileSpace.SharedKernel.Enums;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyFileSpace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [MyFileSpaceAuthorize(RoleType.Admin)]
    public class CacheController : ControllerBase
    {
        private readonly ICacheService _cacheService;

        public CacheController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        // PUT api/<CacheController>/usage
        [HttpGet("usage")]
        public async Task<string> GetMemoryUsage()
        {
            return await _cacheService.GetMemoryUsed();
        }

        // DELETE api/<CacheController>
        [HttpDelete("clear")]
        public async Task Delete()
        {
            await _cacheService.ClearCache();
        }
    }
}
