using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using MyFileSpace.Core.Helpers;
using MyFileSpace.Core.Services;
using MyFileSpace.Core.Services.Implementation;

namespace MyFileSpace.Core
{
    public static class CoreModule
    {
        public static void RegisterCoreServices(this IServiceCollection services)
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
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IAuthService, JwtAuthorizationService>();
            services.AddScoped<Session>();
        }
    }
}
