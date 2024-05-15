using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class FavoriteFileRepository : RepositoryBase<FavoriteFile>, IFavoriteFileRepository
    {
        public FavoriteFileRepository(MyFileSpaceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
