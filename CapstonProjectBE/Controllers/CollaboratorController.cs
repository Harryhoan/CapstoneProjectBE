using Application.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CapstonProjectBE.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class CollaboratorController : Controller
    {
        private readonly ICollaboratorService _collaboratorService;
        private readonly IAuthenService _authenService;
        public CollaboratorController(ICollaboratorService collaboratorService, IAuthenService authenService)
        {
            _collaboratorService = collaboratorService;
            _authenService = authenService;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> CreateCollaborator(int userId, int projectId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _collaboratorService.CreateCollaborator(userId, projectId, user);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Staff, Admin")]
        [HttpGet("pagination/all")]
        public async Task<IActionResult> GetPaginatedCollaborators(int page = 1, int pageSize = 20)
        {
            var result = await _collaboratorService.GetPaginatedCollaborators(page, pageSize);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Staff, Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetCollaborators()
        {
            var result = await _collaboratorService.GetCollaborators();
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        [HttpGet("project")]
        public async Task<IActionResult> GetCollaboratorsByProjectId(int projectId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _collaboratorService.CheckIfUserHasPermissionsByProjectId(projectId, user);
            if (check != null)
            {
                return check;
            }
            var result = await _collaboratorService.GetCollaboratorsByProjectId(projectId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("pagination/project")]
        public async Task<IActionResult> GetPaginatedCollaboratorsByProjectId(int projectId, int page = 1, int pageSize = 20)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _collaboratorService.CheckIfUserHasPermissionsByProjectId(projectId, user);
            if (check != null)
            {
                return check;
            }
            var result = await _collaboratorService.GetPaginatedCollaboratorsByProjectId(projectId, page, pageSize);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        [HttpGet("user")]
        public async Task<IActionResult> GetCollaboratorsByUserId(int userId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var result = await _collaboratorService.GetCollaboratorsByUserId(userId, user);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("pagination/user")]
        public async Task<IActionResult> GetPaginatedCollaboratorsByUserId(int userId, int page = 1, int pageSize = 20)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var result = await _collaboratorService.GetPaginatedCollaboratorsByUserId(userId, page, pageSize, user);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> GetPaginatedCollaboratorsByCurrentUser(int page = 1, int pageSize = 20)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _collaboratorService.GetPaginatedCollaboratorsByUserId(user.UserId, page, pageSize, user);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        [Authorize(Roles = "Customer")]
        [HttpDelete]
        public async Task<IActionResult> RemoveCollaborator(int userId, int projectId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _collaboratorService.CheckIfUserCanRemoveByProjectId(projectId, user);
            if (check != null)
            {
                return check;
            }
            var result = await _collaboratorService.RemoveCollaborator(userId, projectId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
