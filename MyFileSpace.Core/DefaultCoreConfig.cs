﻿using Microsoft.Extensions.DependencyInjection;
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
            services.AddScoped<IFileManagementService, FileManagementService>();
            services.AddScoped<ICacheService, CacheService>();
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
