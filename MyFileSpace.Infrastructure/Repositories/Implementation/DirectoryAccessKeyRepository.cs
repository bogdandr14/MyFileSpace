using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class DirectoryAccessKeyRepository : RepositoryBase<DirectoryAccessKey>, IDirectoryAccessKeyRepository
    {
        public DirectoryAccessKeyRepository(MyFileSpaceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
