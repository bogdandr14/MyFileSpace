using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Core.Specifications
{
    internal class OwnedFilesSpec : Specification<StoredFile>, ISpecification<StoredFile>, ISingleResultSpecification<StoredFile>
    {
        public OwnedFilesSpec(Guid ownerId)
        {
            Query.Where(x => x.OwnerId.Equals(ownerId))
                .Include(x => x.Directory);
        }

        public OwnedFilesSpec(Guid ownerId, Guid fileId, bool isDeleted = false)
        {
            Query.Where(x => x.OwnerId.Equals(ownerId) && x.Id.Equals(fileId) && x.IsDeleted == isDeleted)
                .Include(x => x.Directory);
        }
    }
}
