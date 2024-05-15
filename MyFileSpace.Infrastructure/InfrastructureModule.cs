using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.Infrastructure.Repositories.Implementation;

namespace MyFileSpace.Infrastructure
{
    public static class InfrastructureModule
    {
        public static void RegisterInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IAccessKeyRepository, AccessKeyRepository>();
            services.AddScoped<IDirectoryAccessKeyRepository, DirectoryAccessKeyRepository>();
            services.AddScoped<IFileAccessKeyRepository, FileAccessKeyRepository>();
            services.AddScoped<IStoredFileRepository, StoredFileRepository>();
            services.AddScoped<IUserDirectoryAccessRepository, UserDirectoryAccessRepository>();
            services.AddScoped<IUserFileAccessRepository, UserFileAccessRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IVirtualDirectoryRepository, VirtualDirectoryRepository>();
            services.AddScoped<IFavoriteFileRepository, FavoriteFileRepository>();
            services.AddScoped<IUserAccessKeyRepository, UserAccessKeyRepository>();
        }
    }
}
