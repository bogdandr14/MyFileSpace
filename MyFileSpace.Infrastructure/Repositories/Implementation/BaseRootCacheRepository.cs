using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using MyFileSpace.Infrastructure.Interfaces;
using MyFileSpace.Caching;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class BaseRootCacheRepository<T, U> : RepositoryBase<T>, IRepositoryBase<T> where T : class, IRootEntity<U>
    {
        private readonly ICacheManager _cacheManager;

        protected string CacheKey(T entity)
        {
            return $"{nameof(T)}_{entity.Id}";
        }

        public BaseRootCacheRepository(MyFileSpaceDbContext dbContext, ICacheManager cacheManager) : base(dbContext)
        {
            _cacheManager = cacheManager;
        }

        public override async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await base.UpdateAsync(entity, cancellationToken);
            await _cacheManager.RemoveAsync(CacheKey(entity));
        }

        public override async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await base.UpdateRangeAsync(entities, cancellationToken);
            foreach (var entity in entities)
            {
                await _cacheManager.RemoveAsync(CacheKey(entity));
            }
        }

        public override async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            await base.DeleteAsync(entity, cancellationToken);
            await _cacheManager.RemoveAsync(CacheKey(entity));
        }

        public override async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await base.DeleteRangeAsync(entities, cancellationToken);
            foreach (var entity in entities)
            {
                await _cacheManager.RemoveAsync(CacheKey(entity));
            }
        }

        public async override Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        {
            Func<Task<T?>> asyncFunc = async () => await base.GetByIdAsync(id, cancellationToken);
            return await _cacheManager.GetAndSetAsync($"{nameof(T)}_{id}", asyncFunc);
        }
    }
}
