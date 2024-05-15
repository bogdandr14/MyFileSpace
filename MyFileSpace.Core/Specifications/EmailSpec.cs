using Ardalis.Specification;
using MyFileSpace.Infrastructure.Entities;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Specifications
{
    internal class EmailSpec : Specification<User>, ISpecification<User>
    {
        public EmailSpec(string email, bool includeUserKeys = false)
        {
            if (includeUserKeys)
            {
                Query.Where(a => a.Email == email).Include(u => u.UserAccessKeys).ThenInclude(uak => uak.AccessKey);
            }
            else
            {
                Query.Where(a => a.Email == email);
            }
        }

        public EmailSpec(string email, string resetKey)
        {
            Query.Where(u => u.Email == email
                        && u.UserAccessKeys.Any(uak => uak.Type == UserKeyType.ResetPassword
                            && uak.AccessKey.Key == resetKey
                            && uak.AccessKey.ExpiresAt.CompareTo(DateTime.UtcNow) > 0));
        }
    }
}
