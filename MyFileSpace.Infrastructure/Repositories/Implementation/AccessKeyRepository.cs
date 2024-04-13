using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure.Persistence;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class AccessKeyRepository : RepositoryBase<AccessKey>, IAccessKeyRepository
    {
        public AccessKeyRepository(MyFileSpaceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
