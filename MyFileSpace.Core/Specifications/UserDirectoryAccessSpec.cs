using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Core.Specifications
{
    internal class UserDirectoryAccessSpec : Specification<UserDirectoryAccess>, ISpecification<UserDirectoryAccess>
    {
        public UserDirectoryAccessSpec(Guid directoryId, List<Guid> userIds)
        {
            Query.Where(ufl => ufl.DirectoryId == directoryId && userIds.Contains(ufl.AllowedUserId));
        }

        public UserDirectoryAccessSpec(Guid directoryId)
        {
            Query.Where(ufl => ufl.DirectoryId == directoryId).Include(ufa => ufa.AllowedUser);
        }
    }
}
