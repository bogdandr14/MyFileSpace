using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Core.Specifications
{
    internal class UsernameSpec : Specification<User>, ISpecification<User>
    {
        public UsernameSpec(string username)
        {
            Query.Where(a => a.Username == username);
        }
    }
}
