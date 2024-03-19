using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyFileSpace.SharedKernel.Helpers;

namespace MyFileSpace.Infrastructure.Persistence
{
    public static class DefaultPersistenceConfig
    {
        public const string USE_CONNECTION = "UseConnection";
        public const string DEFAULT_CONNECTION = "DefaultConnection";

        public static void RegisterDbContext(this IServiceCollection services, IConfiguration configurationManager)
        {
            string connectionToUse = configurationManager.GetConfigValue(USE_CONNECTION) ?? DEFAULT_CONNECTION;
            string? connectionString = configurationManager.GetConnectionString(connectionToUse);

            services.AddDbContext<MyFileSpaceDbContext>(options => options.UseSqlServer(connectionString)); // will be created in web project root
        }

        public static void SetDbInstance(this IServiceProvider serviceProvider)//to use in program main
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<MyFileSpaceDbContext>();

                    //context.Database.Migrate();

                    context.Database.EnsureCreated();
                    SeedData.CheckDbInitialized(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger>();
                    logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
                }
            }
        }
    }
}