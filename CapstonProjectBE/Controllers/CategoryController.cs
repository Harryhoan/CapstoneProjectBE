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
        private readonly IAuthenService _authenService;
        public CategoryController(ICategoryService categoryService, IAuthenService authenService)
        {
            _authenService = authenService;
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

        [Authorize(Roles = "Admin")]
        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromForm] AddCategory category)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null) Unauthorized();
            var newCategory = await _categoryService.AddCategory(user.UserId, category);
            return Ok(newCategory);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null) Unauthorized();
            return Ok(await _categoryService.DeleteCategory(user.UserId, categoryId));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteCategoryFromProject")]
        public async Task<IActionResult> DeleteCategoryFromProject(int projectId, int categoryId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null) Unauthorized();
            return Ok(await _categoryService.DeleteCategoryFromProject(user.UserId, projectId, categoryId));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCate(int categoryId, UpdateCategory updateCate)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null) Unauthorized();
            var updateCategory = await _categoryService.UpdateCategory(user.UserId, categoryId, updateCate);
            return Ok(updateCategory);
        }
    }
}
