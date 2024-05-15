using Ardalis.Specification;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Core.Specifications
{
    internal class FavoriteFileSpec : Specification<FavoriteFile>, ISingleResultSpecification<FavoriteFile>
    {
        public FavoriteFileSpec(Guid fileId, Guid userId)
        {
            Query.Where(ff => ff.FileId.Equals(fileId) && ff.UserId.Equals(userId));
        }
    }
}
