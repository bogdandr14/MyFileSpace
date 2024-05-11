using Ardalis.Specification;
using MyFileSpace.Infrastructure.Persistence.Entities;

namespace MyFileSpace.Core.Specifications
{
    internal class UserWithAccessSpec : Specification<User>, ISpecification<User>
    {
        public UserWithAccessSpec(Guid objectId)
        {
            Query.Where(u => u.IsConfirmed && (u.AllowedFiles.Any(af => af.FileId == objectId) || u.AllowedDirectories.Any(ad => ad.DirectoryId == objectId)));
        }
    }
}
