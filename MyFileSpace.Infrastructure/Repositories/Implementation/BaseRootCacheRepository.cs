using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure.Persistence;
using MyFileSpace.Infrastructure.Persistence.Interfaces;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class BaseRootCacheRepository<T, U> : RepositoryBase<T>, IRepositoryBase<T> where T : class, IRootEntity<U>
    {
        private readonly ICacheRepository _cacheRepository;

        protected string CacheKey(T entity)
        {
            return $"{nameof(T)}_{entity.Id}";
        }

        public BaseRootCacheRepository(MyFileSpaceDbContext dbContext, ICacheRepository cacheRepository) : base(dbContext)
        {
            _cacheRepository = cacheRepository;
        }

        public BaseRootCacheRepository(MyFileSpaceDbContext dbContext, ISpecificationEvaluator specificationEvaluator, ICacheRepository cacheRepository) : base(dbContext, specificationEvaluator)
        {
            _cacheRepository = cacheRepository;
        }


        public override async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await base.UpdateAsync(entity, cancellationToken);
            await _cacheRepository.RemoveAsync(CacheKey(entity));
        }

        public override async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await base.UpdateRangeAsync(entities, cancellationToken);
            foreach (var entity in entities)
            {
                await _cacheRepository.RemoveAsync(CacheKey(entity));
            }
        }

        public override async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            await base.DeleteAsync(entity, cancellationToken);
            await _cacheRepository.RemoveAsync(CacheKey(entity));
        }

        public override async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await base.DeleteRangeAsync(entities, cancellationToken);
            foreach (var entity in entities)
            {
                await _cacheRepository.RemoveAsync(CacheKey(entity));
            }
        }

        public async override Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        {
            Func<Task<T?>> asyncFunc = async () => await base.GetByIdAsync(id, cancellationToken);
            return await _cacheRepository.GetAndSetAsync($"{nameof(T)}_{id}", asyncFunc);
        }
    }
}
