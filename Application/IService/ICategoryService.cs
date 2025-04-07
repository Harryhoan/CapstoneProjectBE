using Application.ServiceResponse;
using Application.ViewModels.CategoryDTO;

namespace Application.IService
{
    public interface ICategoryService
    {
        public Task<ServiceResponse<IEnumerable<ViewCategory>>> GetAllCategory();
        public Task<ServiceResponse<IEnumerable<ViewCategory>>> GetAllCategoryByProjectId(int projectId);
        public Task<ServiceResponse<IEnumerable<ViewCategory>>> GetAllCategoryByParentId(int parentId);
        public Task<ServiceResponse<ViewCategory>> GetCategoryByCategoryId(int categoryId);
        public Task<ServiceResponse<AddCategory>> AddCategory(int userId, AddCategory category);
        public Task<ServiceResponse<ViewCategory>> UpdateCategory(int userId, int categoryId, UpdateCategory updateCategory);
        public Task<ServiceResponse<int>> DeleteCategory(int userId, int categoryId);
        public Task<ServiceResponse<int>> DeleteCategoryFromProject(int userId, int projectId, int categoryId);

    }
}
