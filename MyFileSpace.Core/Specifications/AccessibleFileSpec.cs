using Ardalis.Specification;
using MyFileSpace.Infrastructure.Entities;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Specifications
{
    internal class AccessibleFileSpec : Specification<AccessKey, StoredFile>, ISingleResultSpecification<AccessKey, StoredFile>
    {
        public AccessibleFileSpec(Guid fileId, string accessKey)
        {
            Query.Where(ak => ak.Key == accessKey
                && ((ak.FileAccess != null && ak.FileAccess.FileId == fileId
                        && ak.FileAccess.AccessibleFile.AccessLevel != AccessType.Private && ak.FileAccess.AccessibleFile.IsDeleted == false)
                    || (ak.DirectoryAccess != null && ak.DirectoryAccess.AccessibleDirectory.AccessLevel != AccessType.Private
                        && (ak.DirectoryAccess.AccessibleDirectory.FilesInDirectory.Any(fid => fid.Id == fileId && fid.AccessLevel != AccessType.Private))
                        )
                    )
                );
        }
    }
}
