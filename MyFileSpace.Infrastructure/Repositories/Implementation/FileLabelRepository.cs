using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure.Persistence;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class FileLabelRepository : RepositoryBase<FileLabel>, IFileLabelRepository
    {
        public FileLabelRepository(MyFileSpaceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
