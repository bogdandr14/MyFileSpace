using MyFileSpace.Infrastructure.Persistence;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class UserRepository : BaseRootCacheRepository<User, Guid>, IUserRepository
    {
        public UserRepository(MyFileSpaceDbContext dbContext, ICacheRepository cacheRepository) : base(dbContext, cacheRepository)
        {
        }
    }
}
