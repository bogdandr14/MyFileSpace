namespace MyFileSpace.SharedKernel.Repositories
{
    public interface ICacheRepository
    {
        public IEnumerable<string> GetAllKeys();
        public bool IsObjectCached(string key);
        public T? Get<T>(string key);
        public T GetAndSet<T>(string key, Func<T> fallback, TimeSpan? timespan = null);
        public void Set<T>(string key, T value, TimeSpan? timeSpan = null);
        public byte[]? GetBytes(string key);
        public byte[] GetAndSetBytes(string key, Func<byte[]> fallback, TimeSpan? timespan = null);
        public void SetBytes(string key, byte[] bytes, TimeSpan? timeSpan = null);
        public void Remove(string key);
        public void Clear();
    }
}
