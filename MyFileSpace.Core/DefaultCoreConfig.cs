using Microsoft.Extensions.DependencyInjection;
using MyFileSpace.Core.Services;
using MyFileSpace.Core.Services.Implementation;
using MyFileSpace.Infrastructure;

namespace MyFileSpace.Core
{
    public static class DefaultCoreConfig
    {
        public static void RegisterCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IFileManagementService, FileManagementService>();
            services.AddScoped<ICacheService, CacheService>();
            services.RegisterInfrastructureServices();
        }
    }
}
