using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure.Persistence;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class LabelRepository : RepositoryBase<Label>, ILabelRepository
    {
        public LabelRepository(MyFileSpaceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
