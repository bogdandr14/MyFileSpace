using Ardalis.Specification;
using MyFileSpace.Infrastructure.Entities;

namespace MyFileSpace.Core.Specifications
{
    internal class UserFavoritesSpec : Specification<StoredFile>, ISpecification<StoredFile>
    {
        public UserFavoritesSpec(Guid userId)
        {
            Query.Where(sf => sf.UsersFavorite.Any(uf => uf.UserId.Equals(userId)))
                .Include(sf => sf.UsersFavorite)
                .Include(sf => sf.AllowedUsers);
        }
    }
}
