using Microsoft.Extensions.DependencyInjection;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.SharedKernel.Repositories;

namespace MyFileSpace.Infrastructure
{
    public static class DefaultInfrastructureConfig
    {
        public static void RegisterInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingleton<ICacheRepository, CacheRepository>();
            services.AddScoped<IFileDataRepository, CsvFileDataRepository>();
            services.AddScoped<IFileSystemRepository, FileSystemRepository>();
        }
    }
}
