using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

namespace MyFileSpace.Infrastructure.Caching
{
    internal class MemoryDistributedCacheWrap : MemoryDistributedCache, IMemoryDistributedCacheWrap
    {

        private readonly MemoryDistributedCacheOptions options;

        private MemoryCache cache;

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
            FieldInfo? cacheField = typeof(MemoryDistributedCache).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(x => x.FieldType == typeof(IMemoryCache));
            if (cacheField == null)
            {
                throw new Exception($"Could not find field of type {nameof(IMemoryCache)} on type {nameof(MemoryDistributedCache)}");
            }

            // Find the entries concurrent dictionary on the MemoryCache type.
            FieldInfo? entriesField = typeof(MemoryCache).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(ConcurrentDictionary<,>));
            if (entriesField == null)
            {
                throw new Exception($"Could not find entries concurrent dictionary on type {nameof(MemoryCache)}");
            }

            // Find the cache size field on the the MemoryCache type.
            FieldInfo? cacheSizeField = typeof(MemoryCache).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(x => x.FieldType == typeof(Int64) && x.Name.Contains("cacheSize"));
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

            IDictionary? dictionary = entriesField.GetValue(cache) as IDictionary;
            if (dictionary == null)
            {
                throw new NullReferenceException($"Could not get object of type {nameof(IDictionary)} from {nameof(MemoryCache)}");
            }
            this.entries = dictionary;
        }

        /// <inheritdoc />
        public void Clear()
        {
            // Clear the dictionary.
            this.entries.Clear();

            // Reset the cache size field, but only if we have a size limit.
            if (this.options.SizeLimit != null)
            {
                this.cacheSizeField.SetValue(this.cache, 0);
            }
        }

        /// <inheritdoc />
        public Task ClearAsync(CancellationToken token = default)
        {
            this.Clear();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetAllKeys()
        {
            return this.entries.Keys.OfType<string>().ToList();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetAllKeysAsync(CancellationToken token = default)
        {
            return await Task.Run(() => this.entries.Keys.OfType<string>().ToList(), token);
        }
    }
}
