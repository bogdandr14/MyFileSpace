namespace MyFileSpace.Core.Services
{
    public interface ICacheService
    {
        public IEnumerable<string> GetAllKeys();
        public bool IsObjectCached(string key);
        public string GetMemoryUsed();
        public void ClearCache();
    }
}
