using Application.IService;
using Application.ViewModels.CategoryDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CapstonProjectBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("GetAllCategory")]
        public async Task<IActionResult> GetAllCategory()
        {
            return Ok(await _categoryService.GetAllCategory());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromForm] AddCategory category)
        {
            var newCategory = await _categoryService.AddCategory(category);
            return Ok(newCategory);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            return Ok(await _categoryService.DeleteCategory(categoryId));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCate(int categoryId, UpdateCategory updateCate)
        {
            var updateCategory = await _categoryService.UpdateCategory(categoryId, updateCate);
            return Ok(updateCategory);
        }
    }
}
