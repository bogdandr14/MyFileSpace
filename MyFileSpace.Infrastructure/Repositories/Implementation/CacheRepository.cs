using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyFileSpace.Infrastructure.Caching;
using MyFileSpace.SharedKernel.Helpers;
using Newtonsoft.Json;
using System.Text;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class CacheRepository : ICacheRepository
    {
        private IDistributedCacheWrap _cache;
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

        public async Task<IEnumerable<string>> GetAllKeysAsync()
        {
            return await _cache.GetAllKeysAsync();
        }

        public bool IsObjectCached(string key)
        {
            return !_bypassCache && _cache.Get(key) != null;
        }

        public async Task<bool> IsObjectCachedAsync(string key)
        {
            return !_bypassCache && await _cache.GetAsync(key) != null;
        }

        public T GetAndSet<T>(string key, Func<T> fallback, TimeSpan? timespan = null)
        {
            T? obj = Get<T>(key);
            if (obj is not null)
            {
                return obj;
            }

            obj = fallback.Invoke();
            if (obj is not null)
            {
                Set(key, obj, timespan);
            }

            return obj;
        }

        public async Task<T> GetAndSetAsync<T>(string key, Func<Task<T>> fallback, TimeSpan? timespan = null)
        {
            T? obj = await GetAsync<T>(key);
            if (obj is not null)
            {
                return obj;
            }

            obj = await fallback.Invoke();
            if (obj is not null)
            {
                await SetAsync(key, obj, timespan);
            }

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

        public async Task<T?> GetAsync<T>(string key)
        {
            byte[]? bytes = await GetBytesAsync(key);

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

        public async Task SetAsync<T>(string key, T value, TimeSpan? timeSpan)
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

            await SetBytesAsync(key, bytes, timeSpan);
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

        public async Task<byte[]> GetAndSetBytesAsync(string key, Func<byte[]> fallback, TimeSpan? timespan = null)
        {
            byte[]? obj = await GetBytesAsync(key);
            if (obj is not null)
            {
                return obj;
            }

            obj = fallback.Invoke();

            await SetBytesAsync(key, obj, timespan);

            return obj;
        }

        public byte[]? GetBytes(string key)
        {
            if (_bypassCache)
            {
                return default;
            }

            ValidateAction(key);

            return _cache.Get(key);
        }

        public async Task<byte[]?> GetBytesAsync(string key)
        {
            if (_bypassCache)
            {
                return default;
            }

            ValidateAction(key);

            return await _cache.GetAsync(key);
        }

        public void SetBytes(string key, byte[] bytes, TimeSpan? timeSpan)
        {
            if (_bypassCache)
            {
                return;
            }

            ValidateAction(key);

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

        public async Task SetBytesAsync(string key, byte[] bytes, TimeSpan? timeSpan)
        {
            if (_bypassCache)
            {
                return;
            }

            ValidateAction(key);

            timeSpan ??= _cacheLifeSpan;
            if (timeSpan.HasValue)
            {
                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = timeSpan.Value
                };
                await _cache.SetAsync(key, bytes, options);
            }
            else
            {
                await _cache.SetAsync(key, bytes);
            }
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public string GetMemoryUsed()
        {
            double cacheSizeKB = _cache.GetCacheSize() / 1024;
            return $"{cacheSizeKB}KB used";
        }

        public async Task<string> GetMemoryUsedAsync()
        {
            double cacheSizeKB = await _cache.GetCacheSizeAsync() / 1024;
            return $"{cacheSizeKB}KB used";
        }

        public void Clear()
        {
            _cache.Clear();
        }

        public async Task ClearAsync()
        {
            await _cache.ClearAsync();
        }

        private void ValidateAction(string key)
        {
            if (_cache == null)
            {
                throw new ArgumentNullException(nameof(_cache));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
        }
    }
}
