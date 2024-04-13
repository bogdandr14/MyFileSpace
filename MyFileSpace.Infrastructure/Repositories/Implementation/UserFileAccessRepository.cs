using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure.Persistence;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class UserFileAccessRepository : RepositoryBase<UserFileAccess>, IUserFileAccessRepository
    {
        public UserFileAccessRepository(MyFileSpaceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
