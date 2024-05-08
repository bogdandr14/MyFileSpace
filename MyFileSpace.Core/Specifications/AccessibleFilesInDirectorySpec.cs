using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Specifications
{
    internal class AccessibleFilesInDirectorySpec : Specification<StoredFile>, ISpecification<StoredFile>
    {
        public AccessibleFilesInDirectorySpec(Guid directoryId, Guid userId)
        {
            Query.Where(f =>
                    f.DirectorId == directoryId
                    && f.IsDeleted == false
                    && (f.OwnerId == userId
                        || f.AccessLevel == AccessType.Public
                        || (f.AccessLevel == AccessType.Restricted
                            && (f.AllowedUsers.Any(au => au.AllowedUserId == userId)
                                || (f.Directory.AccessLevel != AccessType.Private
                                    && f.Directory.AllowedUsers.Any(au => au.AllowedUserId == userId))
                                )
                            )
                        )
                    );
        }
        public AccessibleFilesInDirectorySpec(Guid directoryId, Guid userId, string accessKey)
        {
            Query.Where(f =>
                    f.DirectorId == directoryId
                    && f.IsDeleted == false
                    && (f.OwnerId == userId
                        || f.AccessLevel == AccessType.Public
                        || (f.AccessLevel == AccessType.Restricted
                            && ((f.Directory.DirectoryAccessKey != null && f.Directory.DirectoryAccessKey.AccessKey.Key == accessKey)
                                || (f.AllowedUsers.Any(au => au.AllowedUserId == userId)
                                    || (f.Directory.AccessLevel != AccessType.Private
                                        && f.Directory.AllowedUsers.Any(au => au.AllowedUserId == userId))
                                    )
                                )
                            )
                        )
                    );
        }
    }
}
