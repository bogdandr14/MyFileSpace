using MyFileSpace.Infrastructure.Repositories;

namespace MyFileSpace.Core.Services.Implementation
{
    internal class CacheService : ICacheService
    {
        private readonly ICacheRepository _cacheRepository;
        public CacheService(ICacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }
        public async Task<IEnumerable<string>> GetAllKeys()
        {
            return await _cacheRepository.GetAllKeysAsync();
        }

        public async Task<bool> IsObjectCached(string key)
        {
            return await _cacheRepository.IsObjectCachedAsync(key);
        }

        public async Task<string> GetMemoryUsed()
        {
            return await _cacheRepository.GetMemoryUsedAsync();
        }

        public async Task ClearCache()
        {
            await _cacheRepository.ClearAsync();
        }

    }
}
