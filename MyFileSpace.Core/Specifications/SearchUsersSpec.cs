using Ardalis.Specification;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Specifications
{
    internal class SearchUsersSpec : Specification<User>, ISpecification<User>
    {
        public SearchUsersSpec(InfiniteScrollFilter filter, Guid requestUserId)
        {
            Query.Where(user => user.TagName.Contains(filter.Name) && user.Role == RoleType.Customer && !user.Id.Equals(requestUserId))
                .OrderBy(file => file.TagName)
                .Skip(filter.Skip)
                .Take(filter.Take);
        }
    }
}
