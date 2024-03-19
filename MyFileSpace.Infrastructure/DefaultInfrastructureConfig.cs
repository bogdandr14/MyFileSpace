using Microsoft.Extensions.DependencyInjection;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.Infrastructure.Repositories.Implementation;

namespace MyFileSpace.Infrastructure
{
    public static class DefaultInfrastructureConfig
    {
        public static void RegisterInfrastructureServices(this IServiceCollection builder, bool isDevelopment)
        {
            if (isDevelopment)
            {
                RegisterDevelopmentOnlyDependencies(builder);
            }
            else
            {
                RegisterProductionOnlyDependencies(builder);
            }

            RegisterCommonDependencies(builder);
        }

        private static void RegisterCommonDependencies(IServiceCollection services)
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

        private static void RegisterDevelopmentOnlyDependencies(IServiceCollection builder)
        {
            // NOTE: Add any development only services here
            /*builder.RegisterType<FakeEmailSender>().As<IEmailSender>()
              .InstancePerLifetimeScope();*/
        }

        private static void RegisterProductionOnlyDependencies(IServiceCollection builder)
        {
            // NOTE: Add any production only services here
            /* builder.RegisterType<SmtpEmailSender>().As<IEmailSender>()
               .InstancePerLifetimeScope();*/
        }
    }
}
