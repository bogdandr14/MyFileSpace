using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class UserFileAccessRepository : RepositoryBase<UserFileAccess>, IUserFileAccessRepository
    {
        public UserFileAccessRepository(MyFileSpaceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
