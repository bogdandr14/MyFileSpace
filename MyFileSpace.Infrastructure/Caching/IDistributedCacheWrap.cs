using Microsoft.Extensions.Caching.Distributed;

namespace MyFileSpace.Infrastructure.Caching
{
    internal interface IDistributedCacheWrap : IDistributedCache
    {
        void Clear();
        Task ClearAsync(CancellationToken token = default(CancellationToken));

        IEnumerable<string> GetAllKeys();
        Task<IEnumerable<string>> GetAllKeysAsync(CancellationToken token = default(CancellationToken));
        
        long GetCacheSize();
        Task<long> GetCacheSizeAsync();
    }
}
