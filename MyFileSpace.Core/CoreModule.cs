using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyFileSpace.Core.Helpers;
using MyFileSpace.Core.Services;
using MyFileSpace.Core.Services.Implementation;
using MyFileSpace.Core.StorageManager;

namespace MyFileSpace.Core
{
    public static class CoreModule
    {
        public static void RegisterCoreServices(this IServiceCollection services, bool isDevelopment, IConfiguration configuration)
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
            var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAccessKeyService, AccessKeyService>();
            services.AddScoped<IStoredFileService, StoredFileService>();
            services.AddScoped<IUserAccessService, UserAccessService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IVirtualDirectoryService, VirtualDirectoryService>();
            services.AddScoped<IAuthService, JwtAuthorizationService>();
            services.AddScoped<Session>();
        }

        private static void RegisterDevelopmentOnlyDependencies(IServiceCollection services)
        {
            services.AddSingleton<IStorageManager, SystemStorageManager>();
        }

        private static void RegisterProductionOnlyDependencies(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IStorageManager, AzureStorageManager>();
        }
    }
}
