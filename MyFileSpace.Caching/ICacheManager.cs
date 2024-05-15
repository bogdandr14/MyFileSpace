using MyFileSpace.SharedKernel;

namespace MyFileSpace.Caching
{
    public interface ICacheManager
    {
        public Task<IEnumerable<string>> GetAllKeysAsync();
        public Task<bool> IsObjectCachedAsync(string key);
        public Task<T> GetAndSetAsync<T>(string key, Func<Task<T>> fallback, TimeSpan? timespan = null);
        public Task<T> GetAndSetAsync<T>(string key, Func<T> fallback, TimeSpan? timespan = null);
        public Task<T?> GetAsync<T>(string key);
        public Task SetAsync<T>(string key, T value, TimeSpan? timeSpan = null);
        public Task<byte[]> GetAndSetBytesAsync(string key, Func<byte[]> fallback, TimeSpan? timespan = null);
        public Task<byte[]?> GetBytesAsync(string key);
        public Task SetBytesAsync(string key, byte[] bytes, TimeSpan? timeSpan = null);
        public Task RemoveAsync(string key);
        public Task RemoveByPrefixAsync(string prefix);
        public Task<MemorySize> GetMemoryUsedAsync();
        public Task ClearAsync();
    }
}
