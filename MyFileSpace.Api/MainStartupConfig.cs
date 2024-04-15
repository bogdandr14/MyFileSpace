using MyFileSpace.Core;
using MyFileSpace.Infrastructure;
using MyFileSpace.Infrastructure.Persistence;
using MyFileSpace.SharedKernel;
namespace MyFileSpace.Api
{
    internal static class MainStartupConfig
    {
        #region "Builder setup"
        internal static void AddModulesConfiguration(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
        {
            bool isDevelopment = environment.EnvironmentName.Equals(Constants.DEVELOPMENT, StringComparison.OrdinalIgnoreCase);

            services.RegisterSharedKernelServices(isDevelopment, configuration);
            services.RegisterDbContext(configuration);
            services.RegisterInfrastructureServices(isDevelopment, configuration);
            services.RegisterCoreServices();
            services.RegisterApiServices(isDevelopment);
        }
        #endregion

        #region "Application setup"
        internal static void UseSwaggerUIConfiguration(this IApplicationBuilder builder)
        {
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            builder.UseSwaggerUI();
        }
        internal static void UserCorsConfiguration(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseCors(options =>
            {
                options.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        }
        #endregion
    }

}