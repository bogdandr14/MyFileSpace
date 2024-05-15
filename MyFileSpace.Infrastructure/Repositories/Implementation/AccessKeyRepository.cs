using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class AccessKeyRepository : RepositoryBase<AccessKey>, IAccessKeyRepository
    {
        public AccessKeyRepository(MyFileSpaceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
