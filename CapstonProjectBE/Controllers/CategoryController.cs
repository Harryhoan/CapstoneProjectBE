using Application.IService;
using Application.ViewModels.CategoryDTO;
using Domain.Entities;
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

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromForm] AddCategory category)
        {
            var newCategory = await _categoryService.AddCategory(category);
            return Ok(newCategory);
        }
    }
}
