using Microsoft.Extensions.DependencyInjection;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.Infrastructure.Repositories.Implementation;

namespace MyFileSpace.Infrastructure
{
    public static class InfrastructureModule
    {
        public static void RegisterInfrastructureServices(this IServiceCollection services, bool isDevelopment)
        {
            if (isDevelopment)
            {
                RegisterDevelopmentOnlyDependencies(services);
            }
            else
            {
                RegisterProductionOnlyDependencies(services);
            }

            RegisterCommonDependencies(services);
        }

        private static void RegisterCommonDependencies(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSingleton<ICacheRepository, CacheRepository>();
            services.AddScoped<IAccessKeyRepository, AccessKeyRepository>();
            services.AddScoped<IDirectoryAccessKeyRepository, DirectoryAccessKeyRepository>();
            services.AddScoped<IFileAccessKeyRepository, FileAccessKeyRepository>();
            services.AddScoped<IStoredFileRepository, StoredFileRepository>();
            services.AddScoped<IUserDirectoryAccessRepository, UserDirectoryAccessRepository>();
            services.AddScoped<IUserFileAccessRepository, UserFileAccessRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IVirtualDirectoryRepository, VirtualDirectoryRepository>();
        }

        private static void RegisterDevelopmentOnlyDependencies(IServiceCollection services)
        {
            services.AddSingleton<IFileStorageRepository, SystemStorageRepository>();
        }

        private static void RegisterProductionOnlyDependencies(IServiceCollection services)
        {
            services.AddSingleton<IFileStorageRepository, AzureStorageRepository>();
        }
    }
}
