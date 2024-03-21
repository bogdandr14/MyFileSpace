using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Specifications
{
    internal class OwnedFilesSpec : Specification<StoredFile>, ISpecification<StoredFile>, ISingleResultSpecification<StoredFile>
    {
        public OwnedFilesSpec(Guid ownerId)
        {
            Query.Where(x => x.OwnerId.Equals(ownerId) && x.State == true);
        }

        public OwnedFilesSpec(Guid ownerId, AccessType accessType)
        {
            Query.Where(x => x.OwnerId.Equals(ownerId) && x.AccessLevel == accessType && x.State == true);
        }

        public OwnedFilesSpec(Guid ownerId, Guid directoryId)
        {
            Query.Where(x => x.OwnerId.Equals(ownerId) && x.DirectorId.Equals(directoryId) && x.State == true);
        }

        public OwnedFilesSpec(Guid ownerId, Guid fileId, bool isAvailable)
        {
            Query.Where(x => x.OwnerId.Equals(ownerId) && x.Id.Equals(fileId) && x.State == isAvailable);
        }

    }
}
