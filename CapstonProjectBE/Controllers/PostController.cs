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
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null) 
            {
                return Unauthorized();
            }
            var result = await _postService.CreatePost(user.UserId, createPostDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpGet("GetPost")]
        public async Task<IActionResult> GetPostById(int postId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _postService.CheckIfUserHasPermissionsByPostId(user, postId);
            if (check != null) 
            {
                return check;
            }
            var result = await _postService.GetPostById(postId, user == null ? null : user.UserId);
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
            var result = await _postService.GetPostsByProjectId(userId, user.UserId);
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
            var result = await _postService.GetPaginatedPostsByProjectId(userId, page, pageSize, user.UserId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("project")]
        [AllowAnonymous]
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
        [HttpPut("Update")]
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
        [HttpDelete("DeletePost")]
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
