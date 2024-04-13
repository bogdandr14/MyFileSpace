using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Core.Specifications
{
    internal class EmailSpec : Specification<User>, ISpecification<User>
    {
        public EmailSpec(string email)
        {
            Query.Where(a => a.Email == email);
        }
    }
}
