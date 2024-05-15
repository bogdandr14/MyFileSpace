using Ardalis.Specification;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Core.Specifications
{
    internal class OwnedFilesSpec : Specification<StoredFile>, ISpecification<StoredFile>, ISingleResultSpecification<StoredFile>
    {
        public OwnedFilesSpec(Guid ownerId)
        {
            Query.Where(x => x.OwnerId.Equals(ownerId));
        }
    }
}
