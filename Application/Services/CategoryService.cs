using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels.CategoryDTO;
using Application.ViewModels.ProjectDTO;
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
        public async Task<ServiceResponse<ViewCategory>> AddCategory(int userId, AddCategory category)
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
                if (category.ParentCategoryId.HasValue)
                {
                    var parentCategoryId = category.ParentCategoryId.Value;
                    if (parentCategoryId < 1)
                    {
                        response.Success = false;
                        response.Message = "Parent Category ID must be a positive integer.";
                        return response;
                    }
                    var existingCategory = await _unitOfWork.CategoryRepo.GetByIdNoTrackingAsync("CategoryId", parentCategoryId);
                    if (existingCategory == null)
                    {
                        response.Success = false;
                        response.Message = "Parent Category cannot be found.";
                        return response;
                    }
                }
                var newCategory = _mapper.Map<Category>(category);
                newCategory.Name = FormatUtils.CapitalizeWords(FormatUtils.TrimSpacesPreserveSingle(newCategory.Name));
                if (!string.IsNullOrWhiteSpace(newCategory.Description))
                {
                    newCategory.Description = FormatUtils.FormatText(FormatUtils.TrimSpacesPreserveSingle(newCategory.Description));
                }
                await _unitOfWork.CategoryRepo.AddAsync(newCategory);

                response.Data = _mapper.Map<ViewCategory>(newCategory);
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

        public async Task<ServiceResponse<string>> DeleteCategory(int categoryId)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var category = await _unitOfWork.CategoryRepo.GetByIdAsync(categoryId);

                if (category == null)
                {
                    response.Success = false;
                    response.Message = "Category not found.";
                    return response;
                }

                var childCategories = await _unitOfWork.CategoryRepo.GetListByParentCategoryIdAsync(category.CategoryId);
                if (childCategories.Any())
                {
                    foreach (var childCategory in childCategories)
                    {
                        childCategory.ParentCategoryId = null;
                    }

                    await _unitOfWork.CategoryRepo.UpdateAllAsync(childCategories);
                }
                await _unitOfWork.CategoryRepo.RemoveAsync(category);

                response.Success = true;
                response.Message = "Category deleted successfully.";
                response.Data = "Category deleted successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return response;
        }

        public async Task<ServiceResponse<string>> DeleteCategoryFromProject(int projectId, int categoryId)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var category = await _unitOfWork.ProjectCategoryRepo.FindEntityAsync(pc => pc.ProjectId == projectId && pc.CategoryId == categoryId);
                if (category == null)
                {
                    response.Success = false;
                    response.Message = "Category not found for this project.";
                    return response;
                }

                await _unitOfWork.ProjectCategoryRepo.RemoveAsync(category);

                response.Success = true;
                response.Message = "Category removed from the project successfully.";
                response.Data = "Category removed from the project successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return response;
        }

        public async Task<ServiceResponse<IEnumerable<ViewCategory>>> GetAllCategory(string? name = null)
        {
            var response = new ServiceResponse<IEnumerable<ViewCategory>>();

            try
            {
                var result = await _unitOfWork.CategoryRepo.GetAllAsync();
                if (result != null && result.Any())
                {

                    var filteredResult = !string.IsNullOrWhiteSpace(name)
                    ? result.Where(c => c.Name != null && c.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                    : result;

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
                    response.Success = true;
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
        public async Task<ServiceResponse<List<ProjectDto>>> GetAllProjectByCategoryId(int categoryId, User? user = null)
        {
            var response = new ServiceResponse<List<ProjectDto>>();
            try
            {
                var projectCategories = await _unitOfWork.ProjectCategoryRepo.GetAllProjectByCategoryAsync(categoryId);

                if (projectCategories == null || !projectCategories.Any())
                {
                    response.Success = true;
                    response.Message = "No projects found for the given category.";
                    return response;
                }

                var projectList = projectCategories
                    .Where(pc => pc.Project != null)
                    .Select(pc => _mapper.Map<ProjectDto>(pc.Project))
                    .ToList();

                if (user == null)
                {
                    projectList.RemoveAll(p => p.Status == ProjectStatusEnum.DELETED || p.Status == ProjectStatusEnum.INVISIBLE);
                }
                else if (user.Role == UserEnum.CUSTOMER)
                {
                    projectList.RemoveAll(p => p.Status == ProjectStatusEnum.DELETED || (p.Status == ProjectStatusEnum.INVISIBLE && user.UserId != p.CreatorId));
                }

                response.Data = projectList;
                response.Success = true;
                response.Message = "Projects retrieved successfully.";
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
                    response.Success = true;
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

        private async Task<bool> HasCircularDependency(Category category, int? parentCategoryId)
        {
            if (parentCategoryId == null) return false;

            var visited = new HashSet<int> { category.CategoryId };
            var currentId = parentCategoryId;
            var categories = await _unitOfWork.CategoryRepo.GetAllAsync();
            while (currentId != null)
            {
                if (visited.Contains(currentId.Value)) return true;

                visited.Add(currentId.Value);
                var parent = categories.FirstOrDefault(c => c.CategoryId == currentId.Value);
                currentId = parent?.ParentCategoryId;
            }

            return false;
        }

        public async Task<ServiceResponse<IEnumerable<ViewCategory>>> GetAllCategoryByProjectId(int projectId)
        {
            var response = new ServiceResponse<IEnumerable<ViewCategory>>();

            try
            {
                var projectCategories = await _unitOfWork.ProjectCategoryRepo.GetListByProjectIdAsync(projectId);

                if (projectCategories == null || !projectCategories.Any())
                {
                    response.Success = true;
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

                if (result.ParentCategoryId.HasValue)
                {
                    var existingParentCategory = await _unitOfWork.CategoryRepo.GetByIdNoTrackingAsync("CategoryId", result.ParentCategoryId.Value);
                    if (existingParentCategory == null)
                    {
                        result.ParentCategoryId = null;
                        await _unitOfWork.CategoryRepo.UpdateAsync(result);
                    }
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
                response.Message = "Get Category successfully";

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

            try
            {
                var result = await _unitOfWork.CategoryRepo.GetByIdAsync(categoryId);
                if (result == null)
                {
                    response.Success = false;
                    response.Message = "Category not found.";
                    return response;
                }

                if (updateCategory.ParentCategoryId.HasValue)
                {
                    var parentCategoryId = updateCategory.ParentCategoryId.Value;
                    if (parentCategoryId < 1)
                    {
                        response.Success = false;
                        response.Message = "Parent Category ID must be a positive integer.";
                        return response;
                    }
                    var existingCategory = await _unitOfWork.CategoryRepo.GetByIdNoTrackingAsync("CategoryId", parentCategoryId);
                    if (existingCategory == null)
                    {
                        response.Success = false;
                        response.Message = "Parent Category cannot be found.";
                        return response;
                    }
                    if (result.ParentCategoryId.HasValue && await HasCircularDependency(result, result.ParentCategoryId.Value))
                    {
                        throw new InvalidOperationException("Circular dependency detected.");
                    }
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
