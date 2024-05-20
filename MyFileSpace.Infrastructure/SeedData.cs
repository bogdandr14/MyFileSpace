using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyFileSpace.Infrastructure.Entities;
using MyFileSpace.SharedKernel;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Infrastructure
{
    internal class SeedData
    {
        private static readonly Lazy<SeedData> _seedData = new Lazy<SeedData>(() => new SeedData());

        private SeedData() { }

        internal static void CheckDbInitialized(IServiceProvider serviceProvider)
        {
            using (var dbContext = new MyFileSpaceDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<MyFileSpaceDbContext>>()))
            {
                if (dbContext.User.Any())
                {
                    return;   // DB has been seeded
                }

                _seedData.Value.PopulateTestData(dbContext);
            }
        }

        private void PopulateTestData(MyFileSpaceDbContext dbContext)
        {
            foreach (var item in dbContext.User)
            {
                dbContext.Remove(item);
            }

            dbContext.SaveChanges();

            User admin = new User();

            admin.Role = RoleType.Admin;
            admin.TagName = "FileSpaceAdmin";
            admin.Password = CryptographyUtility.HashKey("%file*SpacE20-24", out string salt);
            admin.Email = "myfilespacedebug@gmail.com";
            admin.Salt = salt;
            dbContext.User.Add(admin);

            dbContext.SaveChanges();

            VirtualDirectory rootDirectory = new VirtualDirectory()
            {
                AccessLevel = AccessType.Private,
                OwnerId = admin.Id,
                VirtualPath = "$USER_ROOT",
            };
            dbContext.VirtualDirectory.Add(rootDirectory);
            dbContext.SaveChanges();
        }
    }
}
