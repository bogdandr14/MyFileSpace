using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Specifications
{
    internal class AllowedDirectorySpec : Specification<VirtualDirectory>, ISingleResultSpecification<VirtualDirectory>
    {
        public AllowedDirectorySpec(Guid directoryId, Guid userId, bool noIncludes = false)
        {
            if (noIncludes)
            {
                Query.Where(vd =>
                    vd.Id == directoryId
                    && vd.IsDeleted == false
                    && (vd.OwnerId == userId
                        || vd.AccessLevel == AccessType.Public
                        || (vd.AccessLevel == AccessType.Restricted
                            && vd.AllowedUsers.Any(au => au.AllowedUserId == userId)
                            )
                        )
                    );
            }
            else
            {

                Query.Where(vd =>
                   vd.Id == directoryId
                    && vd.IsDeleted == false
                    && (vd.OwnerId == userId
                        || vd.AccessLevel == AccessType.Public
                        || (vd.AccessLevel == AccessType.Restricted
                            && vd.AllowedUsers.Any(au => au.AllowedUserId == userId)
                            )
                        )
                    )
                    .Include(x => x.Owner)
                    .Include(x => x.FilesInDirectory).ThenInclude(x => x.UsersFavorite)
                    .Include(x => x.ChildDirectories)
                    .Include(x => x.AllowedUsers).ThenInclude(x => x.AllowedUser)
                    .Include(x => x.DirectoryAccessKey).ThenInclude(x => x!.AccessKey);
            }
        }

        public AllowedDirectorySpec(Guid directoryId, string accessKey)
        {
            Query.Where(d => d.Id == directoryId && d.IsDeleted == false
                && (d.DirectoryAccessKey != null && d.DirectoryAccessKey.AccessKey.Key == accessKey
                    && d.DirectoryAccessKey.AccessKey.ExpiresAt.CompareTo(DateTime.UtcNow) > 0 && d.AccessLevel != AccessType.Private
                    )
                )
                .Include(x => x.Owner)
                .Include(x => x.AllowedUsers).ThenInclude(x => x.AllowedUser)
                .Include(x => x.FilesInDirectory)
                .Include(x => x.ChildDirectories);
        }
    }
}
