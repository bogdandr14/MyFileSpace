using Ardalis.Specification;
using MyFileSpace.SharedKernel.Entities;

namespace MyFileSpace.Infrastructure.Persistence.Interfaces
{
    public interface IRootEntity<TId> : IEntity<TId>, IGenericEntity
    {
    }
}
