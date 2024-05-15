using Ardalis.Specification;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Core.Specifications
{
    internal class UserFileAccessSpec : Specification<UserFileAccess>, ISpecification<UserFileAccess>
    {
        public UserFileAccessSpec(Guid fileId, List<Guid> userIds)
        {
            Query.Where(ufl => ufl.FileId == fileId && userIds.Contains(ufl.AllowedUserId));
        }

        public UserFileAccessSpec(Guid fileId)
        {
            Query.Where(ufl => ufl.FileId == fileId).Include(ufa => ufa.AllowedUser);
        }
    }
}
