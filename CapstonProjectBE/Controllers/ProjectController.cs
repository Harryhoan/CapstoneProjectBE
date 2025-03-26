using Application.IService;
using Application.Services;
using Application.ViewModels.ProjectDTO;
using Domain.Entities;
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
        [HttpGet("GetAllProject")]
        public async Task<IActionResult> GetAllProject()
        {
            return Ok(await _projectService.GetAllProjects());
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
        [Authorize(Roles = "Customer")]
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
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateProject(CreateProjectDto projectDto)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(await _projectService.CreateProject(user.UserId, projectDto));
        }

        [HttpPut("UpdateProject")]
        [Authorize(Roles = "Customer, Staff, Admin")]
        public async Task<IActionResult> UpdateProject(int projectId, UpdateProjectDto updateProjectDto)
        {
            return Ok(await _projectService.UpdateProject(projectId, updateProjectDto));
        }

        [HttpPut("UpdateProjectThumbnail")]
        [Authorize(Roles = "Customer")]
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
        [Authorize(Roles = "Customer")]
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
        [Authorize(Roles = "Customer, Staff, Admin")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            return Ok(await _projectService.DeleteProject(id));
        }

        [HttpPut("StaffApproveProject")]
        [Authorize(Roles = "Staff, Admin")]
        public async Task<IActionResult> StaffApproveProject(int projectId, bool isApproved, string reason)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(await _projectService.StaffApproveAsync(projectId, user.UserId, isApproved, reason));
        }
    }
}
