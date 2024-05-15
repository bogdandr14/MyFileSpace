using Ardalis.Specification;
using MyFileSpace.Infrastructure.Entities;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Specifications
{
    internal class AllowedFileSpec : Specification<StoredFile>, ISingleResultSpecification<StoredFile>, ISpecification<StoredFile>
    {
        public AllowedFileSpec(Guid fileId, Guid userId)
        {
            Query.Where(f =>
                f.Id == fileId
                && f.IsDeleted == false
                && (f.OwnerId == userId
                    || f.AccessLevel == AccessType.Public
                    || (f.AccessLevel == AccessType.Restricted
                        && (f.AllowedUsers.Any(au => au.AllowedUserId == userId)
                            || f.Directory.AccessLevel != AccessType.Private
                                && f.Directory.AllowedUsers.Any(au => au.AllowedUserId == userId)
                            )
                        )
                    )
                ).Include(f => f.Owner)
                .Include(f => f.UsersFavorite)
                .Include(x => x.AllowedUsers).ThenInclude(x => x.AllowedUser)
                .Include(x => x.FileAccessKey).ThenInclude(x => x!.AccessKey);
        }

        public AllowedFileSpec(Guid fileId, string accessKey)
        {
            Query.Where(f => f.Id == fileId && f.AccessLevel != AccessType.Private && f.IsDeleted == false
                && ((f.FileAccessKey != null && f.FileAccessKey.AccessKey.Key == accessKey)
                    || (f.Directory.DirectoryAccessKey != null && f.Directory.DirectoryAccessKey.AccessKey.Key == accessKey
                        && f.Directory.AccessLevel != AccessType.Private))
                ).Include(f => f.Owner);
        }

    }
}
