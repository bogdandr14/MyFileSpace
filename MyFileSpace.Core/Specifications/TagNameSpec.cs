using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Core.Specifications
{
    internal class TagNameSpec : Specification<User>, ISingleResultSpecification<User>
    {
        public TagNameSpec(string tagName)
        {
            Query.Where(a => a.TagName == tagName && a.IsConfirmed);
        }
    }
}
