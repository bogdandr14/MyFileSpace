
using Ardalis.Specification;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Core.Specifications
{
    internal class DirectoryPathInParentDirectorySpec : Specification<VirtualDirectory>, ISpecification<VirtualDirectory>
    {
        public DirectoryPathInParentDirectorySpec(Guid directoryId, string path)
        {
            Query.Where(x => x.ParentDirectoryId.Equals(directoryId) && x.VirtualPath == path);
        }
    }
}
