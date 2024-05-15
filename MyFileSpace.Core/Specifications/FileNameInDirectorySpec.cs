using Ardalis.Specification;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Core.Specifications
{
    internal class FileNameInDirectorySpec : Specification<StoredFile>, ISpecification<StoredFile>
    {
        public FileNameInDirectorySpec(Guid directoryId, string fileName)
        {
            Query.Where(x => x.DirectorId.Equals(directoryId) && x.Name == fileName);
        }
    }
}
