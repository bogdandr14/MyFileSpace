using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

namespace MyFileSpace.Infrastructure.Caching
{
    internal class MemoryDistributedCacheWrap : MemoryDistributedCache, IDistributedCacheWrap
    {

        private readonly MemoryDistributedCacheOptions options;

        private MemoryCache cache;

        private Object coherentState;

        private IDictionary entries;

        private FieldInfo cacheSizeField;

        public MemoryDistributedCacheWrap(IOptions<MemoryDistributedCacheOptions> optionsAccessor)
            : base(optionsAccessor)
        {
            this.options = optionsAccessor.Value;

            this.GetFields();
        }

        private void GetFields()
        {
            // Find the IMemoryCache field on the MemoryDistributedCache type.
            FieldInfo[] fieldInfos = typeof(MemoryDistributedCache).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo? cacheField = fieldInfos.FirstOrDefault(x => x.FieldType == typeof(IMemoryCache));
            cacheField = fieldInfos.FirstOrDefault(x => x.FieldType == typeof(MemoryCache));
            if (cacheField == null)
            {
                throw new Exception($"Could not find field of type {nameof(IMemoryCache)} on type {nameof(MemoryDistributedCache)}");
            }

            // Find the coherent state for MemoryCache type.
            var coherentStateField = typeof(MemoryCache).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(x => x.Name == "_coherentState");
            if (coherentStateField == null)
            {
                throw new Exception($"Could not find coherent state for {nameof(MemoryCache)}");
            }

            // Find the entries concurrent dictionary on the MemoryCache type.
            FieldInfo[] entriesFields = coherentStateField.FieldType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo? entriesField = entriesFields.FirstOrDefault(x => x.FieldType.GetGenericTypeDefinition() == typeof(ConcurrentDictionary<,>));
            if (entriesField == null)
            {
                throw new Exception($"Could not find entries concurrent dictionary on type {nameof(MemoryCache)}");
            }

            // Find the cache size field on the the MemoryCache type.
            FieldInfo[] cacheSizeFields = coherentStateField.FieldType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo? cacheSizeField = cacheSizeFields.FirstOrDefault(x => x.FieldType == typeof(Int64) && x.Name.Contains("cacheSize"));
            if (cacheSizeField == null)
            {
                throw new Exception($"Could not find cacheSize field on type {nameof(MemoryCache)}");
            }
            this.cacheSizeField = cacheSizeField;

            MemoryCache? memoryCache = cacheField.GetValue(this) as MemoryCache;
            if (memoryCache == null)
            {
                throw new NullReferenceException($"Could not get object of type {nameof(MemoryCache)} from {nameof(MemoryDistributedCache)}");
            }
            this.cache = memoryCache;

            Object? coherent = coherentStateField.GetValue(cache);
            if (coherent == null)
            {
                throw new NullReferenceException($"Could not get object of CoherentState from {nameof(MemoryCache)}");
            }
            this.coherentState = coherent;

            IDictionary? dictionary = entriesField.GetValue(coherentState) as IDictionary;
            if (dictionary == null)
            {
                throw new NullReferenceException($"Could not get object of type {nameof(IDictionary)} from {nameof(MemoryCache)}");
            }

            this.entries = dictionary;

            long? cacheSize = this.cacheSizeField.GetValue(this.coherentState) as long?;
            if (cacheSize == null)
            {
                throw new NullReferenceException($"Could not get cache size from {nameof(MemoryCache)}");
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            // Clear the dictionary.
            this.entries.Clear();

            // Reset the cache size field, but only if we have a size limit.
            if (this.options.SizeLimit != null)
            {
                this.cacheSizeField.SetValue(this.coherentState, 0);
            }
        }

        /// <inheritdoc />
        public async Task ClearAsync(CancellationToken token = default)
        {
            await Task.Run(() => Clear(), token);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetAllKeys()
        {
            return this.entries.Keys.OfType<string>().ToList();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetAllKeysAsync(CancellationToken token = default)
        {
            return await Task.Run(() => GetAllKeys(), token);
        }

        public long GetCacheSize()
        {
            long? cacheSize = cacheSizeField.GetValue(this.coherentState) as long?;

            return cacheSize ?? 0;
        }

        public async Task<long> GetCacheSizeAsync()
        {
            return await Task.FromResult(GetCacheSize());
        }
    }
}
