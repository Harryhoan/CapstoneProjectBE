using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories
{
    public interface IGenericRepo<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        public Task<List<T>> GetAllAsNoTrackingAsync();
        public Task<T?> GetByIdNoTrackingAsync(string primaryKeyName, Guid id);
        public Task<T?> GetByIdNoTrackingAsync(string primaryKeyName, int id);
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task Remove(T entity);
        Task DeleteTokenAsync(T entity);

    }
}
