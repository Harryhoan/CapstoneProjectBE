using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels.CategoryDTO;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<AddCategory>> AddCategory(int userId, AddCategory category)
        {
            var response = new ServiceResponse<AddCategory>();
            var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }
            if (user.Role != UserEnum.ADMIN)
            {
                response.Success = false;
                response.Message = "You do not have permission to update this category.";
                return response;
            }
            try
            {
                var newCategory = _mapper.Map<Category>(category);
                await _unitOfWork.CategoryRepo.AddAsync(newCategory);

                response.Data = category;
                response.Success = true;
                response.Message = "Category created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return response;
        }

        public async Task<ServiceResponse<int>> DeleteCategory(int userId, int categoryId)
        {
            var response = new ServiceResponse<int>();
            var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }
            if (user.Role != UserEnum.ADMIN)
            {
                response.Success = false;
                response.Message = "You do not have permission to update this category.";
                return response;
            }
            try
            {
                var category = await _unitOfWork.CategoryRepo.GetByIdAsync(categoryId);

                if (category == null)
                {
                    response.Success = false;
                    response.Message = "Category not found.";
                    return response;
                }


                await _unitOfWork.CategoryRepo.RemoveAsync(category);

                response.Success = true;
                response.Message = "Category deleted successfully.";
                response.Data = categoryId;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return response;
        }

        public async Task<ServiceResponse<int>> DeleteCategoryFromProject(int userId, int projectId, int categoryId)
        {
            var response = new ServiceResponse<int>();
            var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }
            if (user.Role != UserEnum.ADMIN)
            {
                response.Success = false;
                response.Message = "You do not have permission to update this category.";
                return response;
            }
            try
            {
                var category = await _unitOfWork.ProjectCategoryRepo.FindEntityAsync(pc => pc.ProjectId == projectId && pc.CategoryId == categoryId);
                if (category == null)
                {
                    response.Success = false;
                    response.Message = "No categories found for this project.";
                    return response;
                }

                await _unitOfWork.ProjectCategoryRepo.RemoveAsync(category);

                response.Success = true;
                response.Message = "Categories deleted successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return response;
        } 

        public async Task<ServiceResponse<IEnumerable<ViewCategory>>> GetAllCategory()
        {
            var response = new ServiceResponse<IEnumerable<ViewCategory>>();

            try
            {
                var result = await _unitOfWork.CategoryRepo.GetAllAsync();
                if (result != null && result.Any())
                {
                    response.Data = result.Select(c => new ViewCategory
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name,
                        Description = c.Description,
                        ParentCategoryId = c.ParentCategoryId,
                    });
                    response.Success = true;
                    response.Message = "Categories retrieved successfully.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "No categories found.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return response;
        }

        public async Task<ServiceResponse<IEnumerable<ViewCategory>>> GetAllCategoryByParentId(int parentId)
        {
            var response = new ServiceResponse<IEnumerable<ViewCategory>>();

            try
            {
                var categories = await _unitOfWork.CategoryRepo.GetListByParentCategoryIdAsync(parentId);

                if (categories == null || !categories.Any())
                {
                    response.Success = false;
                    response.Message = "No Categories found for the given Project.";
                    return response;
                }

                if (categories.Any())
                {
                    response.Data = categories.Select(c => new ViewCategory
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name,
                        Description = c.Description,
                        ParentCategoryId = c.ParentCategoryId,
                    });
                    response.Success = true;
                    response.Message = "Categories retrieved successfully.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "No categories found.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<ViewCategory>>> GetAllCategoryByProjectId(int projectId)
        {
            var response = new ServiceResponse<IEnumerable<ViewCategory>>();

            try
            {
                var projectCategories = await _unitOfWork.ProjectCategoryRepo.GetListByProjectIdAsync(projectId);

                if (projectCategories == null || !projectCategories.Any())
                {
                    response.Success = false;
                    response.Message = "No ProjectCategories found for the given Project.";
                    return response;
                }

                var categories = projectCategories
                    .Where(pc => pc.Category != null)
                    .Select(pc => pc.Category)
                    .Where(c => c != null)
                    .ToList();

                if (categories.Any())
                {
                    response.Data = categories.Select(c => new ViewCategory
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name,
                        Description = c.Description,
                        ParentCategoryId = c.ParentCategoryId,
                    });
                    response.Success = true;
                    response.Message = "Categories retrieved successfully.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "No categories found.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return response;
        }

        public async Task<ServiceResponse<ViewCategory>> GetCategoryByCategoryId(int categoryId)
        {
            var response = new ServiceResponse<ViewCategory>();

            try
            {
                var result = await _unitOfWork.CategoryRepo.GetByIdAsync(categoryId);
                if (result == null)
                {
                    response.Success = false;
                    response.Message = "Category not found.";
                    return response;
                }

                var category = new ViewCategory
                {
                    CategoryId = categoryId,
                    Name = result.Name,
                    Description = result.Description,
                    ParentCategoryId = result.ParentCategoryId,
                };
                response.Data = category;
                response.Success = true;
                response.Message = "Category updated successfully";

                return response;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return response;
        }

        public async Task<ServiceResponse<ViewCategory>> UpdateCategory(int userId, int categoryId, UpdateCategory updateCategory)
        {
            var response = new ServiceResponse<ViewCategory>();
            var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }
            if (user.Role != UserEnum.ADMIN)
            {
                response.Success = false;
                response.Message = "You do not have permission to update this category.";
                return response;
            }
            try
            {
                var result = await _unitOfWork.CategoryRepo.GetByIdAsync(categoryId);
                if (result == null)
                {
                    response.Success = false;
                    response.Message = "Category not found.";
                    return response;
                }

                var updateCate = _mapper.Map<Category>(updateCategory);
                await _unitOfWork.CategoryRepo.UpdateAsync(updateCate);

                var category = new ViewCategory
                {
                    CategoryId = categoryId,
                    Name = updateCategory.Name,
                    Description = updateCategory.Description,
                    ParentCategoryId = updateCate.ParentCategoryId,
                };
                response.Data = category;
                response.Success = true;
                response.Message = "Category updated successfully";

                return response;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return response;
        }
    }
}
