using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MyFileSpace.Infrastructure.Persistence
{
    public static class PersistenceSetup
    {
        public static void AddDbContext(this IServiceCollection services, string connectionString) =>
           services.AddDbContext<MyFileSpaceDbContext>(options =>
               options.UseSqlServer(connectionString)); // will be created in web project root

        public static void SetDbInstance(this IServiceProvider serviceProvider)
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