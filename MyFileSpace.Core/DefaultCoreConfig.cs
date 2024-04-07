using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using MyFileSpace.Core.Helpers;
using MyFileSpace.Core.Services;
using MyFileSpace.Core.Services.Implementation;

namespace MyFileSpace.Core
{
    public static class DefaultCoreConfig
    {
        public static void RegisterCoreServices(this IServiceCollection builder, bool isDevelopment)
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

        public static void RegisterCommonDependencies(this IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.AddScoped<IAccessKeyService, AccessKeyService>();
            services.AddScoped<IStoredFileService, StoredFileService>();
            services.AddScoped<IUserAccessService, UserAccessService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IVirtualDirectoryService, VirtualDirectoryService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IAuthService, JwtAuthorizationService>();
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
