using Application.IService;
using Application.ViewModels.ProjectDTO;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;


namespace CapstonProjectBE.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IAuthenService _authenService;
        public ProjectController(IProjectService projectService, IAuthenService authenService)
        {
            _projectService = projectService;
            _authenService = authenService;
        }

        /// <summary>
        /// Get all project
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllProject")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProject([FromQuery] QueryProjectDto? queryProjectDto)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var result = await _projectService.GetAllProjects(user, queryProjectDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Retrieves all projects associated with an admin or staff user after verifying their authorization.
        /// </summary>
        /// <returns>Returns an Ok response with the list of projects if the user is authorized.</returns>
        [HttpGet("GetAllProjectByMonitor")]
        [Authorize(Roles = "ADMIN, STAFF")]
        public async Task<IActionResult> GetAllProjectByMonitor()
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null) { return Unauthorized(); }
            var result = await _projectService.GetAllProjectByAdminAsync(user.UserId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("GetProjectsPaging")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProjectsPaging(int pageNumber, int pageSize, [FromQuery] QueryProjectDto? queryProjectDto)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var result = await _projectService.GetProjectsPaging(pageNumber, pageSize, user, queryProjectDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("GetProjectById")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _authenService.CheckIfUserCanGetByProjectId(id, user);
            if (check != null)
            {
                return check;
            }
            var result = await _projectService.GetProjectById(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("GetProjectByUserId")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetProjectByUserId()
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _projectService.GetProjectByUserIdAsync(user.UserId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("CreateProject")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CreateProject([FromForm] CreateProjectDto projectDto)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _projectService.CreateProject(user.UserId, projectDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);

        }

        [HttpPut("UpdateProject")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> UpdateProject(int projectId, [FromForm] UpdateProjectDto updateProjectDto)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _authenService.CheckIfUserHasPermissionsToUpdateOrDeleteByProjectId(projectId, user);
            if (check != null)
            {
                return check;
            }
            var result = await _projectService.UpdateProject(projectId, updateProjectDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateProjectThumbnail")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> UpdateProjectThumbnail(int projectId, IFormFile file)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _projectService.UpdateProjectThumbnail(projectId, file);
            if (!result.Success)
            {
                return BadRequest();
            }
            return Ok(result);
        }
        [HttpPut("UpdateProjectStory")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> UpdateProjectStory(int projectId, string story)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _authenService.CheckIfUserHasPermissionsToUpdateOrDeleteByProjectId(projectId, user);
            if (check != null)
            {
                return check;
            }
            var result = await _projectService.UpdateProjectStoryAsync(projectId, story);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpDelete("DeleteProject")]
        [Authorize(Roles = "CUSTOMER, ADMIN, STAFF")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _authenService.CheckIfUserHasPermissionsToUpdateOrDeleteByProjectId(id, user);
            if (check != null)
            {
                return check;
            }
            var result = await _projectService.DeleteProject(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "STAFF, ADMIN")]
        [HttpPut("StaffApproveProject")]
        public async Task<IActionResult> StaffApproveProject(int projectId, ProjectStatusEnum status, string reason)
        {
            if (status == ProjectStatusEnum.DELETED)
            {
                var message = "Invalid status: DELETED is not allowed.";
                return BadRequest(message);
            }
            if (status != ProjectStatusEnum.REJECTED && status != ProjectStatusEnum.APPROVED)
            {
                var message = "Invalid status: Only APPROVED and REJECTED are allowed.";
                return BadRequest(message);
            }
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var check = await _authenService.CheckIfUserHasPermissionsToUpdateOrDeleteByProjectId(projectId, user);
            if (check != null)
            {
                return check;
            }
            var result = await _projectService.StaffApproveAsync(projectId, user.UserId, status, reason);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "CUSTOMER, STAFF")]
        [HttpPost("AddCategoryToProject")]
        public async Task<IActionResult> AddCategoryToProject([FromForm] AddCategoryToProject addCategoryToProject)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _authenService.CheckIfUserHasPermissionsToUpdateOrDeleteByProjectId(addCategoryToProject.ProjectId, user);
            if (check != null)
            {
                return check;
            }
            var result = await _projectService.AddCategoryToProject(addCategoryToProject);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("ChangeProjectMonitor")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ChangeProjectMonitor(int projectId, int staffId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _projectService.ChangeProjectMonitorAsync(projectId, staffId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
