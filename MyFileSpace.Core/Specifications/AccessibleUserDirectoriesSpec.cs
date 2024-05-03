using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Specifications
{
    internal class AccessibleUserDirectoriesSpec : Specification<VirtualDirectory>, ISpecification<VirtualDirectory>
    {
        // returns all directories of the owner which are accessible by the requester
        public AccessibleUserDirectoriesSpec(Guid ownerUserId, Guid requestingUser)
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

        // returns all directories that the requester was granted access to
        public AccessibleUserDirectoriesSpec(Guid requestingUser)
        {
            Query.Where(vd => vd.IsDeleted == false
                && vd.AccessLevel != AccessType.Private
                && vd.AllowedUsers.Any(ad => ad.AllowedUserId == requestingUser)
            );
        }
    }
}
