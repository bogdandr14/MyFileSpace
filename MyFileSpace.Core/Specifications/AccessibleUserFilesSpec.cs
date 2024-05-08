using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Specifications
{
    internal class AccessibleUserFilesSpec : Specification<StoredFile>, ISpecification<StoredFile>
    {
        // returns all files of the owner which are accessible by the requester
        public AccessibleUserFilesSpec(Guid ownerUserId, Guid requestingUser)
        {
            Query.Where(vd => vd.IsDeleted == false
                && vd.OwnerId == ownerUserId
                && (vd.AccessLevel == AccessType.Public
                    || (vd.AccessLevel == AccessType.Restricted
                        && vd.AllowedUsers.Any(ad => ad.AllowedUserId == requestingUser)
                        )
                    )
            );
        }

        // returns all files that the requester was granted access to
        public AccessibleUserFilesSpec(Guid requestingUser)
        {
            Query.Where(vd => vd.IsDeleted == false
                && (vd.AccessLevel != AccessType.Private
                && vd.AllowedUsers.Any(ad => ad.AllowedUserId == requestingUser)
                )
            );
        }
    }
}
