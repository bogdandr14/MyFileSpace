using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Specifications
{
    internal class UserWithAccessKeySpec : Specification<User>, ISpecification<User>, ISingleResultSpecification<User>
    {
        public UserWithAccessKeySpec(string accessKey, UserKeyType userKeyType)
        {
            Query.Where(u => u.UserAccessKeys.Any(uak => uak.Type == userKeyType
                                && uak.AccessKey.Key.Equals(accessKey)
                                && uak.AccessKey.ExpiresAt.CompareTo(DateTime.UtcNow) > 0)
                    );
        }
    }
}
