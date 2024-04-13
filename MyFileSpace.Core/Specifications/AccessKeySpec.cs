using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Specifications
{
    internal class AccessKeySpec : Specification<AccessKey>, ISingleResultSpecification<AccessKey>
    {
        public AccessKeySpec(Guid objectId, ObjectType objectType)
        {
            if (objectType == ObjectType.StoredFile)
            {
                Query.Where(x => x.FileAccess != null && x.FileAccess.FileId == objectId);
            }
            if (objectType == ObjectType.VirtualDirectory)
            {
                Query.Where(x => x.DirectoryAccess != null && x.DirectoryAccess.DirectoryId == objectId);
            }
        }

        public AccessKeySpec(Guid objectId, ObjectType objectType, Guid ownerId)
        {
            if (objectType == ObjectType.StoredFile)
            {
                Query.Where(x => x.FileAccess != null && x.FileAccess.FileId.Equals(objectId) && x.FileAccess.AccessibleFile.OwnerId.Equals(ownerId));
            }
            if (objectType == ObjectType.VirtualDirectory)
            {
                Query.Where(x => x.DirectoryAccess != null && x.DirectoryAccess.DirectoryId.Equals(objectId) && x.DirectoryAccess.AccessibleDirectory.OwnerId.Equals(ownerId));
            }
        }
    }
}
