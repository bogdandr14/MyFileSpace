using Microsoft.AspNetCore.Mvc;
using MyFileSpace.Core.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyFileSpace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly ICacheService _cacheService;

        public CacheController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        // GET: api/<CacheController>
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            return await _cacheService.GetAllKeys();
        }

        // GET api/<CacheController>/contains
        [HttpGet("contains/{key}")]
        public async Task<bool> IsCached(string key)
        {
            return await _cacheService.IsObjectCached(key);
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
