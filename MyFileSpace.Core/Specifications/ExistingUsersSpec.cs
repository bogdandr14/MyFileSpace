using Ardalis.Specification;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Core.Specifications
{
    internal class ExistingUsersSpec : Specification<User>, ISpecification<User>
    {
        public ExistingUsersSpec(List<Guid> userIds)
        {
            Query.Where(u => userIds.Contains(u.Id));
        }
    }
}
