using Application.IService;
using Application.ViewModels.PostDTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;

namespace CapstonProjectBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly IAuthenService _authenService;
        public PostController(IPostService postService, IAuthenService authenService)
        {
            _postService = postService;
            _authenService = authenService;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostDTO createPostDTO)
        {
            var result = await _postService.CreatePost(createPostDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetPostsByUserId(int userId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var result = user == null || (user.Role == "Customer" && userId != user.UserId) ? await _postService.GetPostsByUserId(userId, false) : await _postService.GetPostsByUserId(userId, true);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("pagination/user")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPaginatedPostsByUserId(int userId, int page = 1, int pageSize = 20)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var result = user == null || (user.Role == "Customer" && userId != user.UserId) ? await _postService.GetPaginatedPostsByUserId(userId, page, pageSize, false) : await _postService.GetPaginatedPostsByUserId(userId, page, pageSize, true);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [Authorize]
        [HttpGet("project")]
        public async Task<IActionResult> GetPostsByProjectId(int projectId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var result = user == null ? await _postService.GetPostsByProjectId(projectId, null) : await _postService.GetPostsByProjectId(projectId, user.UserId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("pagination/project")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPaginatedPostsByProjectId(int projectId, int page = 1, int pageSize = 20)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var result = user == null ? await _postService.GetPaginatedPostsByProjectId(projectId, page, pageSize, null) : await _postService.GetPaginatedPostsByProjectId(projectId, page, pageSize, user.UserId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        [Authorize(Roles = "Customer")]
        [HttpPut]
        public async Task<IActionResult> UpdatePost(int postId, CreatePostDTO createPostDTO)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.Role == "Customer" && !(await _postService.CheckIfPostHasUserId(postId, user.UserId)))
            {
                return Forbid();
            }
            var result = await _postService.UpdatePost(postId, createPostDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Customer, Staff")]
        [HttpDelete]
        public async Task<IActionResult> RemovePost(int postId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.Role == "Customer" && !(await _postService.CheckIfPostHasUserId(postId, user.UserId)))
            {
                return Forbid();
            }
            var result = await _postService.RemovePost(postId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
