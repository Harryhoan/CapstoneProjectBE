using Application.IService;
using Application.ViewModels.CommentDTO;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CapstonProjectBE.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IAuthenService _authenService;
        public CommentController(ICommentService commentService, IAuthenService authenService)
        {
            _commentService = commentService;
            _authenService = authenService;
        }
        [Authorize]
        [HttpPost("post")]
        public async Task<IActionResult> CreatePostComment([FromForm] CreatePostCommentDTO createPostCommentDTO)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.IsDeleted || !user.IsVerified)
            {
                return Forbid();
            }
            var check = await _commentService.CheckIfUserHasPermissionsByPostId(user, createPostCommentDTO.PostId);
            if (check != null)
            {
                return check;
            }
            var result = await _commentService.CreatePostComment(createPostCommentDTO, user);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("project")]
        public async Task<IActionResult> CreateProjectComment([FromForm] CreateProjectCommentDTO createProjectCommentDTO)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.IsDeleted || !user.IsVerified)
            {
                return Forbid();
            }
            var check = await _commentService.CheckIfUserHasPermissionsByProjectId(user, createProjectCommentDTO.ProjectId);
            if (check != null)
            {
                return check;
            }
            var result = await _commentService.CreateProjectComment(createProjectCommentDTO, user);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "ADMIN, STAFF")]
        [HttpGet("all")]
        public async Task<IActionResult> GetComments()
        {
            var result = await _commentService.GetComments();
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        [HttpGet("GetComment")]
        public async Task<IActionResult> GetCommentsByPostId(int postId)
        {
            var result = await _commentService.GetCommentsByPostId(postId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("pagination")]
        public async Task<IActionResult> GetPaginatedCommentsByPostId(int postId, int page = 1, int pageSize = 20)
        {
            var result = await _commentService.GetPaginatedCommentsByPostId(postId, page, pageSize);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("GetCommentsByProjectId")]
        public async Task<IActionResult> GetCommentsByProjectId(int projectId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _authenService.CheckIfUserCanGetByProjectId(projectId, user);
            if (check != null)
            {
                return check;
            }
            var result = await _commentService.GetCommentsByProjectId(projectId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("project/count")]
        public async Task<IActionResult> GetCountCommentByProjectId(int projectId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _authenService.CheckIfUserCanGetByProjectId(projectId, user);
            if (check != null)
            {
                return check;
            }
            var result = await _commentService.GetCommentCountByProjectId(projectId, user);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpGet("post/count")]
        public async Task<IActionResult> GetCountCommentByPostId(int postId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var result = await _commentService.GetCommentCountByPostId(postId, user);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }



        [HttpGet("pagination/project")]
        public async Task<IActionResult> GetPaginatedCommentsByProjectId(int projectId, int page = 1, int pageSize = 20)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _authenService.CheckIfUserCanGetByProjectId(projectId, user);
            if (check != null)
            {
                return check;
            }
            var result = await _commentService.GetPaginatedCommentsByProjectId(projectId, page, pageSize);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpGet("GetCommentByUserId")]
        public async Task<IActionResult> GetCommentsByUserId(int userId)
        {
            var result = await _commentService.GetCommentsByUserId(userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("pagination/user")]
        public async Task<IActionResult> GetPaginatedCommentsByUserId(int userId, int page = 1, int pageSize = 20)
        {
            var result = await _commentService.GetPaginatedCommentsByUserId(userId, page, pageSize);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "CUSTOMER")]
        [HttpPut("UpdateComment")]
        public async Task<IActionResult> UpdateComment([FromForm] UpdateCommentDTO updateCommentDTO)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            if (!(await _commentService.CheckIfCommentHasUserId(updateCommentDTO.CommentId, user.UserId)))
            {
                return Forbid();
            }
            if (user.IsDeleted || !user.IsVerified)
            {
                return Forbid();
            }
            var result = await _commentService.UpdateComment(updateCommentDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "CUSTOMER, STAFF")]
        [HttpDelete("DeleteComment")]
        public async Task<IActionResult> RemoveComment(int commentId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.Role == UserEnum.CUSTOMER && !(await _commentService.CheckIfCommentHasUserId(commentId, user.UserId)))
            {
                return Forbid();
            }
            if (user.IsDeleted || !user.IsVerified)
            {
                return Forbid();
            }
            var result = await _commentService.RemoveComment(commentId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "CUSTOMER, STAFF")]
        [HttpDelete("SoftDelComment")]
        public async Task<IActionResult> SoftRemoveComment(int commentId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.Role == UserEnum.CUSTOMER && !(await _commentService.CheckIfCommentHasUserId(commentId, user.UserId)))
            {
                return Forbid();
            }
            var result = await _commentService.SoftRemoveComment(commentId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

    }
}