using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Core.Specifications
{
    internal class OwnedRootDirectorySpec : Specification<VirtualDirectory>, ISingleResultSpecification<VirtualDirectory>
    {
        public OwnedRootDirectorySpec(Guid ownerId)
        {
            Query.Where(x => x.OwnerId.Equals(ownerId) && x.ParentDirectoryId == null);
        }
    }
}
