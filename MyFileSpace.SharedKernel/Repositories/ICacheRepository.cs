namespace MyFileSpace.SharedKernel.Repositories
{
    public interface ICacheRepository
    {
        public IEnumerable<string> GetAllKeys();
        public bool IsObjectCached(string key);
        public T? Get<T>(string key);
        public T GetAndSet<T>(string key, Func<T> fallback, TimeSpan? timespan = null);
        public void Set<T>(string key, T value);
        public void Set<T>(string key, T value, TimeSpan? timeSpan);
        public void Remove(string key);
        public void Clear();
    }
}
