using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Core.Specifications
{
    internal class OwnedDirectoriesSpec : Specification<VirtualDirectory>, ISingleResultSpecification<VirtualDirectory>
    {
        public OwnedDirectoriesSpec(Guid ownerId, bool rootDirectoryOnly = false)
        {
            Query.Where(x => x.OwnerId.Equals(ownerId) && (!rootDirectoryOnly || x.ParentDirectoryId == null));
        }

        public OwnedDirectoriesSpec(Guid ownerId, Guid directoryId)
        {
            Query.Where(x => x.OwnerId.Equals(ownerId) && x.Id.Equals(directoryId) && x.IsDeleted == false);
        }

        public OwnedDirectoriesSpec(Guid ownerId, Guid directoryId, bool isDeleted)
        {
            Query.Where(x => x.OwnerId.Equals(ownerId) && x.Id.Equals(directoryId) && x.IsDeleted == isDeleted)
                .Include(x=> x.ChildDirectories)
                .Include(x=> x.FilesInDirectory)
                .Include(x => x.AllowedUsers).ThenInclude(x => x.AllowedUser)
                .Include(x => x.DirectoryAccessKey).ThenInclude(x => x!.AccessKey);
        }
    }
}
