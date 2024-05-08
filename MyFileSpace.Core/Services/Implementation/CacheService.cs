using MyFileSpace.Core.DTOs;
using MyFileSpace.Infrastructure.Repositories;

namespace MyFileSpace.Core.Services.Implementation
{
    internal class CacheService : ICacheService
    {
        private readonly ICacheRepository _cacheRepository;
        private readonly string[] _sizeScale = { "B", "KB", "MB", "GB" };
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

        public async Task<MemorySizeDTO> GetMemoryUsed()
        {
            double size = await _cacheRepository.GetMemoryUsedAsync();

            int i = 0;
            while (size > 1024)
            {
                ++i; size /= 1024;
            }
            return new MemorySizeDTO { Scale = _sizeScale[i], Size = size };
        }

        public async Task ClearCache()
        {
            await _cacheRepository.ClearAsync();
        }

    }
}
