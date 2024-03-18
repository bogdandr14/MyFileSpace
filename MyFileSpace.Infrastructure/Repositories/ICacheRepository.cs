namespace MyFileSpace.Infrastructure.Repositories
{
    public interface ICacheRepository
    {
        public IEnumerable<string> GetAllKeys();
        public Task<IEnumerable<string>> GetAllKeysAsync();
        public bool IsObjectCached(string key);
        public Task<bool> IsObjectCachedAsync(string key);
        public T GetAndSet<T>(string key, Func<T> fallback, TimeSpan? timespan = null);
        public Task<T> GetAndSetAsync<T>(string key, Func<Task<T>> fallback, TimeSpan? timespan = null);
        public T? Get<T>(string key);
        public Task<T?> GetAsync<T>(string key);
        public void Set<T>(string key, T value, TimeSpan? timeSpan = null);
        public Task SetAsync<T>(string key, T value, TimeSpan? timeSpan = null);
        public byte[] GetAndSetBytes(string key, Func<byte[]> fallback, TimeSpan? timespan = null);
        public Task<byte[]> GetAndSetBytesAsync(string key, Func<byte[]> fallback, TimeSpan? timespan = null);
        public byte[]? GetBytes(string key);
        public Task<byte[]?> GetBytesAsync(string key);
        public void SetBytes(string key, byte[] bytes, TimeSpan? timeSpan = null);
        public Task SetBytesAsync(string key, byte[] bytes, TimeSpan? timeSpan = null);
        public void Remove(string key);
        public Task RemoveAsync(string key);
        public string GetMemoryUsed();
        public Task<string> GetMemoryUsedAsync();
        public void Clear();
        public Task ClearAsync();
    }
}
