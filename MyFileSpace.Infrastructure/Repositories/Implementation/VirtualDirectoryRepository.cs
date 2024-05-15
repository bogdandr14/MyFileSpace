using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class VirtualDirectoryRepository : RepositoryBase<VirtualDirectory>, IVirtualDirectoryRepository
    {
        public VirtualDirectoryRepository(MyFileSpaceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
