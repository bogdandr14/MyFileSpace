using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure.Persistence;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class DirectoryAccessKeyRepository : RepositoryBase<DirectoryAccessKey>, IDirectoryAccessKeyRepository
    {
        public DirectoryAccessKeyRepository(MyFileSpaceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
