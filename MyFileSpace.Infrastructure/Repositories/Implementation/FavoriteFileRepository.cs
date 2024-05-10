using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure.Persistence;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class FavoriteFileRepository : RepositoryBase<FavoriteFile>, IFavoriteFileRepository
    {
        public FavoriteFileRepository(MyFileSpaceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
