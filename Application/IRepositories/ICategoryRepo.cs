using Domain.Entities;

namespace Application.IRepositories
{
    public interface ICategoryRepo : IGenericRepo<Category>
    {
        Task<IEnumerable<Category>> GetListByParentCategoryIdAsync(int parentCategoryId);
    }
}
