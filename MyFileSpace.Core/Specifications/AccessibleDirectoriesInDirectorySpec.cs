using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Specifications
{
    internal class AccessibleDirectoriesInDirectorySpec : Specification<VirtualDirectory>, ISpecification<VirtualDirectory>
    {
        public AccessibleDirectoriesInDirectorySpec(Guid directoryId, Guid userId)
        {
            Query.Where(vd =>
                 vd.ParentDirectoryId == directoryId
                 && vd.IsDeleted == false
                 && (vd.OwnerId == userId
                     || vd.AccessLevel == AccessType.Public
                     || (vd.AccessLevel == AccessType.Restricted
                         && vd.AllowedUsers.Any(au => au.AllowedUserId == userId)
                         )
                     )
                 );
        }
    }
}
