using Application.ServiceResponse;
using Application.ViewModels.CategoryDTO;

namespace Application.IService
{
    public interface ICategoryService
    {
        public Task<ServiceResponse<IEnumerable<ViewCategory>>> GetAllCategory();
        public Task<ServiceResponse<IEnumerable<ViewCategory>>> GetAllCategoryByProjectId(int projectId);
        public Task<ServiceResponse<AddCategory>> AddCategory(AddCategory category);
        public Task<ServiceResponse<ViewCategory>> UpdateCategory(int categoryId, UpdateCategory updateCategory);
        public Task<ServiceResponse<int>> DeleteCategory(int categoryId);
        public Task<ServiceResponse<int>> DeleteCategoryFromProject(int projectId, int categoryId);

    }
}
