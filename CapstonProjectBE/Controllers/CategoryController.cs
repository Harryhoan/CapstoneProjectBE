using Application.IService;
using Application.ViewModels.CategoryDTO;
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

        [HttpGet("GetCategoryByCategoryId")]
        public async Task<IActionResult> GetCategoryByCategoryId(int categoryId)
        {
            return Ok(await _categoryService.GetCategoryByCategoryId(categoryId));
        }

        [HttpGet("GetCategoryByParentCategoryId")]
        public async Task<IActionResult> GetCategoryByParentCategoryId(int parentCategoryId)
        {
            return Ok(await _categoryService.GetAllCategoryByParentId(parentCategoryId));
        }

        [HttpGet("GetAllCategoryByProjectId")]
        public async Task<IActionResult> GetAllCategoryByProjectId(int projecId)
        {
            return Ok(await _categoryService.GetAllCategoryByProjectId(projecId));
        }

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromForm] AddCategory category)
        {
            var newCategory = await _categoryService.AddCategory(category);
            return Ok(newCategory);
        }

        [HttpDelete("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            return Ok(await _categoryService.DeleteCategory(categoryId));
        }

        [HttpDelete("DeleteCategoryFromProject")]
        public async Task<IActionResult> DeleteCategoryFromProject(int projectId, int categoryId)
        {
            return Ok(await _categoryService.DeleteCategoryFromProject(projectId, categoryId));
        }

        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCate(int categoryId, UpdateCategory updateCate)
        {
            var updateCategory = await _categoryService.UpdateCategory(categoryId, updateCate);
            return Ok(updateCategory);
        }
    }
}
