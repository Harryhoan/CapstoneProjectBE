using Application.IService;
using Application.ViewModels.ProjectDTO;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CapstonProjectBE.Controllers
{
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
        public async Task<IActionResult> GetAllProject()
        {
            return Ok(await _projectService.GetAllProjects());
        }

        /// <summary>
        /// Retrieves all projects associated with an admin or staff user after verifying their authorization.
        /// </summary>
        /// <returns>Returns an Ok response with the list of projects if the user is authorized.</returns>
        [HttpGet("GetAllProjectByMonitor")]
        [Authorize(Roles = "ADMIn, STAFF")]
        public async Task<IActionResult> GetAllProjectByMonitor()
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null) { return Unauthorized(); }
            return Ok(await _projectService.GetAllProjectByAdminAsync(user.UserId));
        }
        [HttpGet("GetProjectsPaging")]
        public async Task<IActionResult> GetProjectsPaging(int pageNumber, int pageSize)
        {
            return Ok(await _projectService.GetProjectsPaging(pageNumber, pageSize));
        }

        [HttpGet("GetProjectById")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            return Ok(await _projectService.GetProjectById(id));
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
            return Ok(await _projectService.GetProjectByUserIdAsync(user.UserId));
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
            return Ok(await _projectService.CreateProject(user.UserId, projectDto));
        }

        [HttpPut("UpdateProject")]
        [Authorize(Roles = "CUSTOMER, STAFF, ADMIN")]
        public async Task<IActionResult> UpdateProject(int projectId, [FromForm] UpdateProjectDto updateProjectDto)
        {
            return Ok(await _projectService.UpdateProject(projectId, updateProjectDto));
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
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(await _projectService.UpdateProjectStoryAsync(user.UserId, projectId, story));
        }
        [HttpDelete("DeleteProject")]
        [Authorize(Roles = "CUSTOMER, ADMIN, STAFF")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            return Ok(await _projectService.DeleteProject(id));
        }

        [HttpPut("StaffApproveProject")]
        [Authorize(Roles = "STAFF, ADMIN")]
        public async Task<IActionResult> StaffApproveProject(int projectId, ProjectEnum status, string reason)
        {
            if (status == ProjectEnum.DELETED)
            {
                return BadRequest("Invalid status: DELETED is not allowed.");
            }
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(await _projectService.StaffApproveAsync(projectId, user.UserId, status, reason));
        }

        [HttpPost("AddCategoryToProject")]
        public async Task<IActionResult> AddCategoryToProject([FromForm] AddCategoryToProject addCategoryToProject)
        {
            return Ok(await _projectService.AddCategoryToProject(addCategoryToProject));
        }
    }
}
