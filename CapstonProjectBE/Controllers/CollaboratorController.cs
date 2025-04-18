using Application.IService;
using Application.ViewModels.CollaboratorDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CapstonProjectBE.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class CollaboratorController : ControllerBase
    {
        private readonly ICollaboratorService _collaboratorService;
        private readonly IAuthenService _authenService;
        public CollaboratorController(ICollaboratorService collaboratorService, IAuthenService authenService)
        {
            _collaboratorService = collaboratorService;
            _authenService = authenService;
        }

        [Authorize(Roles = "CUSTOMER")]
        [HttpPost("CreateCollaborator")]
        public async Task<IActionResult> CreateCollaborator([FromForm] CreateCollaboratorDTO createCollaboratorDTO)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _collaboratorService.CreateCollaborator(createCollaboratorDTO, user);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "CUSTOMER")]
        [HttpPost("CreateCollaboratorByEmail")]
        public async Task<IActionResult> CreateCollaboratorByEmail([FromForm] CreateCollaboratorByEmailDTO createCollaboratorDTO)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _collaboratorService.CreateCollaboratorByUserEmail(createCollaboratorDTO, user);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [Authorize(Roles = "STAFF, ADMIN")]
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

        [Authorize(Roles = "STAFF, ADMIN")]
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
            var check = await _authenService.CheckIfUserCanGetByProjectId(projectId, user);
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
            var check = await _authenService.CheckIfUserCanGetByProjectId(projectId, user);
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

        [Authorize(Roles = "CUSTOMER")]
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
        [Authorize(Roles = "CUSTOMER, STAFF")]
        [HttpPut]
        public async Task<IActionResult> UpdateCollaborator([FromForm] CreateCollaboratorDTO createCollaboratorDTO)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _collaboratorService.CheckIfUserCanUpdateByProjectId(createCollaboratorDTO.Role, createCollaboratorDTO.UserId, createCollaboratorDTO.ProjectId, user);
            if (check != null)
            {
                return check;
            }
            var result = await _collaboratorService.UpdateCollaborator(createCollaboratorDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }



        [Authorize(Roles = "CUSTOMER, STAFF")]
        [HttpDelete]
        public async Task<IActionResult> RemoveCollaborator(int userId, int projectId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _collaboratorService.CheckIfUserCanRemoveByProjectId(userId, projectId, user);
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
