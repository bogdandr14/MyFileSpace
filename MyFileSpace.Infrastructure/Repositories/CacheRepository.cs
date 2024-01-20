using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyFileSpace.Infrastructure.Caching;
using MyFileSpace.Infrastructure.Helpers;
using MyFileSpace.SharedKernel.Repositories;
using Newtonsoft.Json;
using System.Text;

namespace MyFileSpace.Infrastructure.Repositories
{
    internal class CacheRepository : ICacheRepository
    {
        private IMemoryDistributedCacheWrap _cache;
        private readonly TimeSpan? _cacheLifeSpan;
        private bool _bypassCache;

        public CacheRepository(IConfiguration configuration)
        {
            MemoryDistributedCacheOptions distributedCacheOptions = new MemoryDistributedCacheOptions();

            IConfigurationSection cacheConfiguration = configuration.GetConfigSection("DistributedCache");

            bool.TryParse(cacheConfiguration.GetConfigValue("BypassCache"), out _bypassCache);

            if (TimeSpan.TryParse(cacheConfiguration.GetConfigValue("CacheLifeSpan"), out TimeSpan lifeSpan))
            {
                _cacheLifeSpan = lifeSpan;
            }

            if (long.TryParse(cacheConfiguration.GetConfigValue("MemorySizeLimit"), out long sizeLimit))
            {
                distributedCacheOptions.SizeLimit = sizeLimit;
            }

            _cache = new MemoryDistributedCacheWrap(Options.Create(distributedCacheOptions));
        }

        public IEnumerable<string> GetAllKeys()
        {
            return _cache.GetAllKeys();
        }

        public bool IsObjectCached(string key)
        {
            return !_bypassCache && _cache.Get(key) != null;
        }

        public T GetAndSet<T>(string key, Func<T> fallback, TimeSpan? timespan = null)
        {
            T? obj = Get<T>(key);
            if (obj is not null)
            {
                return obj;
            }

            obj = fallback.Invoke();

            Set(key, obj, timespan);

            return obj;
        }

        public T? Get<T>(string key)
        {
            byte[]? bytes = GetBytes(key);

            if (bytes is null)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }

        public void Set<T>(string key, T value, TimeSpan? timeSpan)
        {
            if (_bypassCache)
            {
                return;
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));

            SetBytes(key, bytes, timeSpan);
        }

        public byte[] GetAndSetBytes(string key, Func<byte[]> fallback, TimeSpan? timespan = null)
        {
            byte[]? obj = GetBytes(key);
            if (obj is not null)
            {
                return obj;
            }

            obj = fallback.Invoke();

            SetBytes(key, obj, timespan);

            return obj;
        }

        public byte[]? GetBytes(string key)
        {
            if (_bypassCache)
            {
                return default;
            }

            if (_cache == null)
            {
                throw new ArgumentNullException(nameof(_cache));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _cache.Get(key);
        }

        public void SetBytes(string key, byte[] bytes, TimeSpan? timeSpan)
        {
            if (_bypassCache)
            {
                return;
            }

            if (_cache == null)
            {
                throw new ArgumentNullException(nameof(_cache));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            timeSpan ??= _cacheLifeSpan;
            if (timeSpan.HasValue)
            {

                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = timeSpan.Value
                };
                _cache.Set(key, bytes, options);
            }
            else
            {
                _cache.Set(key, bytes);
            }
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void Clear()
        {
            _cache.Clear();
        }
    }
}
