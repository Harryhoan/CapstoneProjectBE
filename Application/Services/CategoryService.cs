using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels.CategoryDTO;
using Application.ViewModels.ProjectDTO;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Task<ServiceResponse<int>> DeleteCategory(int categoryId)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<IEnumerable<Category>>> GetAllCategory()
        {
            var response = new ServiceResponse<IEnumerable<Category>>();

            try
            {
                var result = await _unitOfWork.CategoryRepo.GetAllAsync();
                if (result != null && result.Any())
                {
                    response.Data = result;
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

        public Task<ServiceResponse<Category>> UpdateCategory()
        {
            throw new NotImplementedException();
        }
    }
}
