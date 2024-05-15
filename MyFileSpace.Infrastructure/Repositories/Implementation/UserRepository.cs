using MyFileSpace.Caching;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class UserRepository : BaseRootCacheRepository<User, Guid>, IUserRepository
    {
        public UserRepository(MyFileSpaceDbContext dbContext, ICacheManager cacheManager) : base(dbContext, cacheManager)
        {
        }
    }
}
