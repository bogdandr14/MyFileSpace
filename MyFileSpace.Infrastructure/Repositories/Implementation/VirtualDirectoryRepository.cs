using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure.Persistence;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class VirtualDirectoryRepository : RepositoryBase<VirtualDirectory>, IVirtualDirectoryRepository
    {
        public VirtualDirectoryRepository(MyFileSpaceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
