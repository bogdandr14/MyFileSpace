using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Specifications
{
    internal class AllowedDirectorySpec : Specification<VirtualDirectory>, ISingleResultSpecification<VirtualDirectory>
    {
        public AllowedDirectorySpec(Guid directoryId, Guid userId)
        {
            Query.Where(vd =>
                vd.Id == directoryId
                && vd.State == true
                && (vd.OwnerId == userId
                    || vd.AccessLevel == AccessType.Public
                    || (vd.AccessLevel == AccessType.Restricted
                        && (vd.Owner.AllowedDirectories.Any(ad =>
                                ad.Directory.AccessLevel != AccessType.Private
                                && ad.AllowedUserId == userId
                                && ad.DirectoryId == directoryId
                                )
                            )
                        )
                    )
                )
                .Include(x => x.Owner)
                .Include(x => x.FilesInDirectory)
                .Include(x => x.ChildDirectories)
                .Include(x => x.AllowedUsers).ThenInclude(x => x.AllowedUser)
                .Include(x => x.DirectoryAccessKey).ThenInclude(x => x.AccessKey);
        }

        public AllowedDirectorySpec(Guid directoryId, string accessKey)
        {
            Query.Where(f => f.Id == directoryId && f.State == true
                && (f.DirectoryAccessKey != null && f.DirectoryAccessKey.AccessKey.Key == accessKey 
                    && f.DirectoryAccessKey.AccessKey.ExpiresAt.CompareTo(DateTime.UtcNow) > 0 && f.AccessLevel != AccessType.Private
                    )
                )
                .Include(x => x.Owner)
                .Include(x => x.FilesInDirectory)
                .Include(x => x.ChildDirectories);
        }
    }
}
