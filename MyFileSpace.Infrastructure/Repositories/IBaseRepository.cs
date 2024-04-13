using Ardalis.Specification;
using MyFileSpace.SharedKernel.Entities;

namespace MyFileSpace.Infrastructure.Repositories
{
    public interface IBaseRepository<T> : IRepositoryBase<T> where T : class, IGenericEntity
    {
    }
}
