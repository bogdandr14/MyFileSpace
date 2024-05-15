using Ardalis.Specification;
using MyFileSpace.SharedKernel.Entities;

namespace MyFileSpace.Infrastructure.Interfaces
{
    public interface IRootEntity<TId> : IEntity<TId>, IGenericEntity
    {
    }
}
