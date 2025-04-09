using Application.IService;
using Application.ViewModels.CategoryDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;


namespace CapstonProjectBE.Controllers
{
    [EnableCors("AllowAll")]
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
            _authenService = authenService;
        }

        [HttpGet("GetAllCategory")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategory()
        {
            var result = await _categoryService.GetAllCategory();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("GetCategoryByCategoryId")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryByCategoryId(int categoryId)
        {
            var result = await _categoryService.GetCategoryByCategoryId(categoryId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("GetAllProjectByCategoryId")]
        public async Task<IActionResult> GetAllProjectByCategoryId(int categoryId)
        {
            return Ok(await _categoryService.GetAllProjectByCategoryId(categoryId));
        }
        [HttpGet("GetCategoryByParentCategoryId")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryByParentCategoryId(int parentCategoryId)
        {
            var result = await _categoryService.GetAllCategoryByParentId(parentCategoryId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("GetAllCategoryByProjectId")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategoryByProjectId(int projecId)
        {
            var result = await _categoryService.GetAllCategoryByProjectId(projecId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize(Roles = "ADMIN, STAFF")]
        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromForm] AddCategory category)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null) Unauthorized();
            var result = await _categoryService.AddCategory(user.UserId, category);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize(Roles = "ADMIN, STAFF")]
        [HttpDelete("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null) Unauthorized();
            var result = await _categoryService.DeleteCategory(user.UserId, categoryId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize(Roles = "ADMIN, CUSTOMER, STAFF")]
        [HttpDelete("DeleteCategoryFromProject")]
        public async Task<IActionResult> DeleteCategoryFromProject(int projectId, int categoryId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _authenService.CheckIfUserHasPermissionsToUpdateOrDeleteByProjectId(projectId, user);
            if (check != null)
            {
                return check;
            }
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _categoryService.DeleteCategoryFromProject(user.UserId, projectId, categoryId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize(Roles = "ADMIN, STAFF")]
        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCate(int categoryId, UpdateCategory updateCate)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null) Unauthorized();
            var result = await _categoryService.UpdateCategory(user.UserId, categoryId, updateCate);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
