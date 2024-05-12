using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure.Persistence;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class UserAccessKeyRepository : RepositoryBase<UserAccessKey>, IUserAccessKeyRepository
    {
        public UserAccessKeyRepository(MyFileSpaceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
