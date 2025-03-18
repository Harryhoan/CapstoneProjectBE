using Application.ServiceResponse;
using Application.ViewModels.CategoryDTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface ICategoryService
    {
        public Task<ServiceResponse<IEnumerable<Category>>> GetAllCategory(); 
        public Task<ServiceResponse<AddCategory>> AddCategory(AddCategory category);
        public Task<ServiceResponse<Category>> UpdateCategory();
        public Task<ServiceResponse<int>> DeleteCategory(int categoryId);

    }
}
