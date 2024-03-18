using Microsoft.Extensions.DependencyInjection;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.Infrastructure.Repositories.Implementation;

namespace MyFileSpace.Infrastructure
{
    public static class DefaultInfrastructureConfig
    {


        public static void RegisterInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingleton<ICacheRepository, CacheRepository>();
            services.AddScoped<IFileDataRepository, CsvFileDataRepository>();
            services.AddScoped<IFileSystemRepository, FileSystemRepository>();
            services.AddScoped<IAccessKeyRepository, AccessKeyRepository>();
            services.AddScoped<IDirectoryAccessKeyRepository, DirectoryAccessKeyRepository>();
            services.AddScoped<IFileAccessKeyRepository, FileAccessKeyRepository>();
            services.AddScoped<IFileLabelRepository, FileLabelRepository>();
            services.AddScoped<ILabelRepository, LabelRepository>();
            services.AddScoped<IStoredFileRepository, StoredFileRepository>();
            services.AddScoped<IUserDirectoryAccessRepository, UserDirectoryAccessRepository>();
            services.AddScoped<IUserFileAccessRepository, UserFileAccessRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IVirtualDirectoryRepository, VirtualDirectoryRepository>();
        }
    }
}
