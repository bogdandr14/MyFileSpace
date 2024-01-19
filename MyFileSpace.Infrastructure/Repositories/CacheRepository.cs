﻿using Microsoft.Extensions.Caching.Distributed;
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

        public T Get<T>(string key, Func<T> fallback)
        {
            T? obj = Get<T>(key);
            if (obj is not null)
            {
                return obj;
            }

            obj = fallback.Invoke();

            Set(key, obj);

            return obj;
        }

        public T? Get<T>(string key)
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

            byte[]? bytes = _cache.Get(key);

            if (bytes is null)
            {
                return default;
            }

            object? obj = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes));

            if (obj is null)
            {
                return default;
            }

            return (T)obj;
        }

        public void Set<T>(string key, T value)
        {
            Set(key, value, _cacheLifeSpan);
        }

        public void Set<T>(string key, T value, TimeSpan? timeSpan)
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

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));

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