using Ardalis.Specification;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Core.Specifications
{
    internal class UserAccessKeySpec : Specification<UserAccessKey>, ISingleResultSpecification<UserAccessKey>
    {
        public UserAccessKeySpec(Guid userId, string accessKey)
        {
            Query.Where(uak => uak.UserId.Equals(userId) && uak.AccessKey.Key.Equals(accessKey)
                    ).Include(uak => uak.AccessKey);
        }
    }
}
