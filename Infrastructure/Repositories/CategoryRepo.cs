using Application.IRepositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CategoryRepo : GenericRepo<Category>, ICategoryRepo
    {
        public CategoryRepo(ApiContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Category>> GetListByParentCategoryIdAsync(int parentCategoryId)
        {
            return await _context.Categories
                .Where(pc => pc.ParentCategoryId.HasValue && pc.ParentCategoryId.Value != pc.CategoryId && pc.ParentCategoryId == parentCategoryId)
                .ToListAsync();
        }
    }
}
