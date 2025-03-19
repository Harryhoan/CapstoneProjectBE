using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories
{
    public interface IGenericRepo<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        public Task<IEnumerable<T>> GetAllAsNoTrackingAsync();
        public Task<T?> GetByIdNoTrackingAsync(string primaryKeyName, Guid id);
        public Task<T?> GetByIdNoTrackingAsync(string primaryKeyName, int id);
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        public Task RemoveAll(IEnumerable<T> entities);
        Task RemoveAsync(T entity);
        Task<bool> Find(Expression<Func<T, bool>> predicate);
    }
}
