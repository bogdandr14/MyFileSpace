using Microsoft.Extensions.DependencyInjection;

namespace MyFileSpace.Caching
{
    public static class CachingModule
    {
        public static void RegisterCaching(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSingleton<ICacheManager, CacheManager>();
        }
    }
}
