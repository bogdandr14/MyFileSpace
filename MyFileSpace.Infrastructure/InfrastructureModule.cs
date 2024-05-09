using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.Infrastructure.Repositories.Implementation;
using MyFileSpace.SharedKernel.Providers;

namespace MyFileSpace.Infrastructure
{
    public static class InfrastructureModule
    {
        public static void RegisterInfrastructureServices(this IServiceCollection services, bool isDevelopment, IConfiguration configuration)
        {
            if (isDevelopment)
            {
                RegisterDevelopmentOnlyDependencies(services);
            }
            else
            {
                RegisterProductionOnlyDependencies(services, configuration);
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
            services.AddScoped<IFavoriteFileRepository, FavoriteFileRepository>();
        }

        private static void RegisterDevelopmentOnlyDependencies(IServiceCollection services)
        {
            services.AddSingleton<IFileStorageRepository, SystemStorageRepository>();
        }

        private static void RegisterProductionOnlyDependencies(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IFileStorageRepository, AzureStorageRepository>();
        }
    }
}
