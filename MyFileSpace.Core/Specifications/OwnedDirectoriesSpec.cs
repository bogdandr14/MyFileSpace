using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Core.Specifications
{
    internal class OwnedDirectoriesSpec : Specification<VirtualDirectory>, ISingleResultSpecification<VirtualDirectory>
    {
        public OwnedDirectoriesSpec(Guid ownerId, Guid directoryId)
        {
            Query.Where(x => x.OwnerId.Equals(ownerId) && x.Id.Equals(directoryId) && x.State == true);
        }
    }
}
