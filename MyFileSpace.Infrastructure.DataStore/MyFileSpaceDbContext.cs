using Microsoft.EntityFrameworkCore;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.Infrastructure.Persistence.Interfaces;
using MyFileSpace.SharedKernel.Entities;
using System;
using System.Reflection;

namespace MyFileSpace.Infrastructure.Persistence
{
    public class MyFileSpaceDbContext : DbContext
    {
        public MyFileSpaceDbContext(DbContextOptions<MyFileSpaceDbContext> options) : base(options)
        {
        }

        #region "DbSets"
        public DbSet<AccessKey> AccessKey => Set<AccessKey>();
        public DbSet<DirectoryAccessKey> DirectoryAccessKey => Set<DirectoryAccessKey>();
        public DbSet<FileAccessKey> FileAccessKey => Set<FileAccessKey>();
        public DbSet<FileLabel> FileLabel => Set<FileLabel>();
        public DbSet<Label> Label => Set<Label>();
        public DbSet<StoredFile> StoredFile => Set<StoredFile>();
        public DbSet<User> User => Set<User>();
        public DbSet<UserDirectoryAccess> UserDirectoryAccess => Set<UserDirectoryAccess>();
        public DbSet<UserFileAccess> UserFileAccess => Set<UserFileAccess>();
        public DbSet<VirtualDirectory> VirtualDirectory => Set<VirtualDirectory>();

        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<VirtualDirectory>().Navigation(p => p.ParentDirectory).AutoInclude();

            modelBuilder.ApplyConfigurationsFromAssembly(
                    Assembly.GetExecutingAssembly(),
                    c => c.Namespace!.Contains(nameof(Configurations))
                        && !c.IsAbstract
                        && c.GetInterfaces().Any(i =>
                            i.IsGenericType
                            && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)
                            && typeof(IGenericEntity).IsAssignableFrom(i.GenericTypeArguments[0])));
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var modifiedEntries = ChangeTracker.Entries()
              .Where(x => x.Entity is IAuditableEntity
                  && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in modifiedEntries)
            {
                IAuditableEntity? entity = entry.Entity as IAuditableEntity;
                if (entity != null)
                {
                    string identityName = Thread.CurrentPrincipal?.Identity?.Name ?? string.Empty;
                    DateTime now = DateTime.UtcNow;

                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedAt = now;
                    }
                    else
                    {
                        base.Entry(entity).Property(x => x.CreatedAt).IsModified = false;
                    }

                    entity.CreatedAt = now;
                }
            }

            int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }

        public override int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}
