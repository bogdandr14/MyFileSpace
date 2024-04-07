using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Specifications
{
    internal class AllowedFileSpec : Specification<StoredFile>, ISingleResultSpecification<StoredFile>
    {
        public AllowedFileSpec(Guid fileId, Guid userId)
        {
            Query.Where(f => 
                f.Id == fileId
                && f.IsDeleted == false
                && (f.OwnerId == userId
                    || f.AccessLevel == AccessType.Public
                    || (f.AccessLevel == AccessType.Restricted 
                        && (f.Owner.AllowedFiles.Any(af => af.FileId == fileId && af.AllowedUserId == userId)
                            || f.Owner.AllowedDirectories.Any(ad => ad.Directory.AccessLevel != AccessType.Private 
                                && ad.AllowedUserId == userId && ad.Directory.FilesInDirectory.Any(fid => fid.Id == fileId)
                                )
                            )
                        )
                    )
                );
        }

        public AllowedFileSpec(Guid fileId, string accessKey)
        {
            Query.Where(f => f.Id == fileId && f.AccessLevel != AccessType.Private && f.IsDeleted == false
                && ((f.FileAccessKey != null && f.FileAccessKey.AccessKey.Key == accessKey)
                    || (f.Directory.DirectoryAccessKey != null && f.Directory.DirectoryAccessKey.AccessKey.Key == accessKey 
                        && f.Directory.AccessLevel != AccessType.Private))
                );
        }

    }
}
