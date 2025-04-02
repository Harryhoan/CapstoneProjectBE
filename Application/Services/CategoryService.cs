using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels.CategoryDTO;
using AutoMapper;
using Domain.Entities;

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
        public async Task<ServiceResponse<AddCategory>> AddCategory(AddCategory category)
        {
            var response = new ServiceResponse<AddCategory>();

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

        public async Task<ServiceResponse<int>> DeleteCategory(int categoryId)
        {
            var response = new ServiceResponse<int>();

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

        public async Task<ServiceResponse<ViewCategory>> UpdateCategory(int categoryId, UpdateCategory updateCategory)
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

                var updateCate = _mapper.Map<Category>(updateCategory);
                updateCate.ParentCategoryId = result.ParentCategoryId;
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
