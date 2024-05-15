using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class UserAccessKeyRepository : RepositoryBase<UserAccessKey>, IUserAccessKeyRepository
    {
        public UserAccessKeyRepository(MyFileSpaceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
