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
        public IEnumerable<string> Get()
        {
            return _cacheService.GetAllKeys();
        }

        // GET api/<CacheController>/contains
        [HttpGet("contains/{key}")]
        public bool IsCached(string key)
        {
            return _cacheService.IsObjectCached(key);
        }

        // PUT api/<CacheController>/usage
        [HttpGet("usage")]
        public string GetMemoryUsage()
        {
            return _cacheService.GetMemoryUsed();
        }

        // DELETE api/<CacheController>
        [HttpDelete("clear")]
        public void Delete()
        {
            _cacheService.ClearCache();
        }
    }
}
