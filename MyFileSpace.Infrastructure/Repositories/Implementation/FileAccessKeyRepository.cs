using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure.Persistence;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class FileAccessKeyRepository : RepositoryBase<FileAccessKey>, IFileAccessKeyRepository
    {
        public FileAccessKeyRepository(MyFileSpaceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
