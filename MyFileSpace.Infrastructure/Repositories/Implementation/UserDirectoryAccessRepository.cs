using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure.Persistence;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class UserDirectoryAccessRepository : RepositoryBase<UserDirectoryAccess>, IUserDirectoryAccessRepository
    {
        public UserDirectoryAccessRepository(MyFileSpaceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
