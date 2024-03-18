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
        public IEnumerable<string> GetAllKeys()
        {
            return _cacheRepository.GetAllKeys();
        }

        public bool IsObjectCached(string key)
        {
            return _cacheRepository.IsObjectCached(key);
        }

        public string GetMemoryUsed()
        {
            return _cacheRepository.GetMemoryUsed();
        }

        public void ClearCache()
        {
            _cacheRepository.Clear();
        }

    }
}
