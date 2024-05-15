using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyFileSpace.Caching.Wrapper;
using MyFileSpace.SharedKernel;
using MyFileSpace.SharedKernel.Providers;
using Newtonsoft.Json;
using System.Text;

namespace MyFileSpace.Caching
{
    internal class CacheManager : ICacheManager
    {
        private IDistributedCacheWrap _cache;
        private readonly TimeSpan? _cacheLifeSpan;
        private bool _bypassCache;
        private readonly string[] _sizeScale = { "B", "KB", "MB", "GB" };


        public CacheManager(IConfiguration configuration)
        {
            MemoryDistributedCacheOptions distributedCacheOptions = new MemoryDistributedCacheOptions();

            bool.TryParse(configuration.GetConfigValue("DistributedCache:BypassCache"), out _bypassCache);

            if (TimeSpan.TryParse(configuration.GetConfigValue("DistributedCache:CacheLifeSpan"), out TimeSpan lifeSpan))
            {
                _cacheLifeSpan = lifeSpan;
            }

            if (long.TryParse(configuration.GetConfigValue("DistributedCache:MemorySizeLimit"), out long sizeLimit))
            {
                distributedCacheOptions.SizeLimit = sizeLimit;
            }

            _cache = new MemoryDistributedCacheWrap(Options.Create(distributedCacheOptions));
        }

        public async Task<IEnumerable<string>> GetAllKeysAsync()
        {
            return await _cache.GetAllKeysAsync();
        }

        public async Task<bool> IsObjectCachedAsync(string key)
        {
            return !_bypassCache && await _cache.GetAsync(key) != null;
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

        public async Task<T> GetAndSetAsync<T>(string key, Func<T> fallback, TimeSpan? timespan = null)
        {
            T? obj = await GetAsync<T>(key);
            if (obj is not null)
            {
                return obj;
            }

            obj = fallback.Invoke();
            if (obj is not null)
            {
                await SetAsync(key, obj, timespan);
            }

            return obj;
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

        public async Task<byte[]?> GetBytesAsync(string key)
        {
            if (_bypassCache)
            {
                return default;
            }

            ValidateAction(key);

            return await _cache.GetAsync(key);
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

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            foreach (string key in (await GetAllKeysAsync()).Where(k => k.StartsWith(prefix)))
            {
                await _cache.RemoveAsync(key);
            }
        }

        public async Task<MemorySize> GetMemoryUsedAsync()
        {
            double size = await _cache.GetCacheSizeAsync();

            int i = 0;
            while (size > 1024)
            {
                ++i; size /= 1024;
            }
            return new MemorySize { Scale = _sizeScale[i], Size = size };
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
