using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class UserDirectoryAccessRepository : RepositoryBase<UserDirectoryAccess>, IUserDirectoryAccessRepository
    {
        public UserDirectoryAccessRepository(MyFileSpaceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
