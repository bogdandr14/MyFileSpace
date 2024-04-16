using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Core.Specifications
{
    internal class OwnedFilesWithDirectoriesSpec : Specification<StoredFile>, ISpecification<StoredFile>, ISingleResultSpecification<StoredFile>
    {
        public OwnedFilesWithDirectoriesSpec(Guid ownerId)
        {
            Query.Where(x => x.OwnerId.Equals(ownerId))
                .Include(x => x.Directory);
        }

        public OwnedFilesWithDirectoriesSpec(Guid ownerId, Guid fileId, bool isDeleted = false)
        {
            Query.Where(x => x.OwnerId.Equals(ownerId) && x.Id.Equals(fileId) && x.IsDeleted == isDeleted)
                .Include(x => x.Directory);
        }
    }
}
