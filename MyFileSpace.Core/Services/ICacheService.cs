using MyFileSpace.Core.DTOs;

namespace MyFileSpace.Core.Services
{
    public interface ICacheService
    {
        public Task<IEnumerable<string>> GetAllKeys();
        public Task<bool> IsObjectCached(string key);
        public Task<MemorySizeDTO> GetMemoryUsed();
        public Task ClearCache();
    }
}
