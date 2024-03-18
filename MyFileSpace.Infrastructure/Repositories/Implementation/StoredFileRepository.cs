using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure.Persistence;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class StoredFileRepository : RepositoryBase<StoredFile>, IStoredFileRepository
    {
        public StoredFileRepository(MyFileSpaceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
