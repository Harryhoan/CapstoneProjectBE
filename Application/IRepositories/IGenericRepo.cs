using System.Linq.Expressions;

namespace Application.IRepositories
{
    public interface IGenericRepo<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        public Task<IEnumerable<T>> GetAllAsNoTrackingAsync();
        public Task<T?> GetByIdNoTrackingAsync(string primaryKeyName, Guid id);
        public Task<T?> GetByIdNoTrackingAsync(string primaryKeyName, int id);
        public IQueryable<T> GetAllAsNoTrackingAsQueryable();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        public Task UpdateAllAsync(IEnumerable<T> entities);
        public Task RemoveAll(IEnumerable<T> entities);
        Task RemoveAsync(T entity);
        Task<bool> Find(Expression<Func<T, bool>> predicate);
        Task<T?> FindEntityAsync(Expression<Func<T, bool>> predicate);
        public Task<int> Count(Expression<Func<T, bool>>? predicate = null);

    }
}
