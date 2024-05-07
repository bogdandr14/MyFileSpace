namespace MyFileSpace.Core.Services
{
    public interface ICacheService
    {
        public Task<IEnumerable<string>> GetAllKeys();
        public Task<bool> IsObjectCached(string key);
        public Task<double> GetMemoryMbUsed();
        public Task ClearCache();
    }
}
