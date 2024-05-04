using Ardalis.Specification;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Specifications
{
    internal class SearchFilesSpec : Specification<StoredFile>, ISpecification<StoredFile>
    {
        public SearchFilesSpec(InfiniteScrollFilter filter)
        {
            Query.Where(file => file.Name.Contains(filter.Name) && file.AccessLevel == AccessType.Public)
                .OrderBy(file => file.Name)
                .Skip(filter.Skip)
                .Take(filter.Take);
        }

        public SearchFilesSpec(InfiniteScrollFilter filter, Guid userId)
        {
            Query.Where(file => file.Name.Contains(filter.Name)
                        && ((file.AccessLevel == AccessType.Public && (filter.IncludeOwn || !file.OwnerId.Equals(userId)))
                            || (file.AccessLevel == AccessType.Restricted && file.AllowedUsers.Any(au => au.AllowedUserId.Equals(userId)))
                            || filter.IncludeOwn && file.OwnerId.Equals(userId))
                        )
                .OrderBy(file => file.Name)
                .Skip(filter.Skip)
                .Take(filter.Take);
        }
    }
}
