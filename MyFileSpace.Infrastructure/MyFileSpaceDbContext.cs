using Microsoft.EntityFrameworkCore;
using MyFileSpace.Infrastructure.Entities;
using MyFileSpace.Infrastructure.Interfaces;
using MyFileSpace.SharedKernel.Entities;
using System.Reflection;

namespace MyFileSpace.Infrastructure
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
        public DbSet<StoredFile> StoredFile => Set<StoredFile>();
        public DbSet<User> User => Set<User>();
        public DbSet<UserDirectoryAccess> UserDirectoryAccess => Set<UserDirectoryAccess>();
        public DbSet<UserFileAccess> UserFileAccess => Set<UserFileAccess>();
        public DbSet<VirtualDirectory> VirtualDirectory => Set<VirtualDirectory>();
        public DbSet<FavoriteFile> FavoriteFile => Set<FavoriteFile>();
        public DbSet<UserAccessKey> UserAccessKey => Set<UserAccessKey>();

        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                    Assembly.GetExecutingAssembly(),
                    c => c.Namespace!.Contains(nameof(Configurations))
                        && !c.IsAbstract
                        && c.GetInterfaces().Any(i =>
                            i.IsGenericType
                            && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)
                            && typeof(IGenericEntity).IsAssignableFrom(i.GenericTypeArguments[0])));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging(); // Enable sensitive data logging in debug mode
#endif
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
                    DateTime now = DateTime.UtcNow;

                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedAt = now;
                    }
                    else
                    {
                        base.Entry(entity).Property(x => x.CreatedAt).IsModified = false;
                    }

                    entity.ModifiedAt = now;
                }

                if (entry.Entity is VirtualDirectory vd)
                {
                    var nowDeleted = (Entry(vd).Property("IsDeleted").CurrentValue as bool?).GetValueOrDefault();
                    var alreadyDeleted = (Entry(vd).Property("IsDeleted").OriginalValue as bool?).GetValueOrDefault();
                    if (nowDeleted && !alreadyDeleted)
                    {
                        var dak = DirectoryAccessKey.FirstOrDefault(c => c.DirectoryId == vd.Id);
                        if (dak != null)
                        {
                            var ak = AccessKey.First(ak => ak.Id == dak.AccessKeyId);
                            DirectoryAccessKey.Remove(dak);
                            AccessKey.Remove(ak);
                        }

                        var uda = UserDirectoryAccess.Where(ufa => ufa.DirectoryId == vd.Id);
                        if (uda.Any())
                        {
                            UserDirectoryAccess.RemoveRange(uda);
                        }
                    }
                }

                if (entry.Entity is StoredFile sf)
                {
                    var nowDeleted = (Entry(sf).Property("IsDeleted").CurrentValue as bool?).GetValueOrDefault();
                    var alreadyDeleted = (Entry(sf).Property("IsDeleted").OriginalValue as bool?).GetValueOrDefault();
                    if (nowDeleted && !alreadyDeleted)
                    {
                        var fak = FileAccessKey.FirstOrDefault(c => c.FileId == sf.Id);
                        if (fak != null)
                        {
                            var ak = AccessKey.First(ak => ak.Id == fak.AccessKeyId);
                            FileAccessKey.Remove(fak);
                            AccessKey.Remove(ak);
                        }

                        var ufa = UserFileAccess.Where(ufa => ufa.FileId == sf.Id);
                        if (ufa.Any())
                        {
                            UserFileAccess.RemoveRange(ufa);
                        }
                    }
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
