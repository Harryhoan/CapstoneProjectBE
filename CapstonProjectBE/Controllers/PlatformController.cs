using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels.PlatformDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CapstonProjectBE.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]

    public class PlatformController : ControllerBase
    {
        private readonly IPlatformService _platformService;
        private readonly IAuthenService _authenService;
        public PlatformController(IPlatformService platformService, IAuthenService authenService)
        {
            _platformService = platformService;
            _authenService = authenService;
        }
        [HttpGet("Platform/GetAll")]
        public async Task<IActionResult> GetAllPlatformAsync()
        {
            var result = await _platformService.GetAllPlatformAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpGet("GetPlatformByProjectId/{projectId}")]
        public async Task<IActionResult> GetAllPlatformByProjectId(int projectId)
        {
            var result = await _platformService.GetAllPlatformByProjectId(projectId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpGet("GetAllProjectByPlatformId/{platformId}")]
        public async Task<IActionResult> GetAllProjectByPlatformId(int platformId)
        {
            var result = await _platformService.GetAllProjectByPlatformId(platformId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("Platform/{search}")]
        public async Task<IActionResult> GetPlatforms(string? query = null)
        {
            var result = await _platformService.GetPlatforms(query);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        [HttpGet("pagination")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPaginatedPlatforms(string? query = null, int page = 1, int pageSize = 20)
        {
            var result = await _platformService.GetPaginatedPlatforms(query, page, pageSize);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize(Roles = "STAFF, ADMIN")]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePlatform([FromForm]CreatePlatformDTO createPlatformDTO)
        {
            var result = await _platformService.CreatePlatform(createPlatformDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize(Roles = "STAFF, ADMIN")]
        [HttpPut]
        public async Task<IActionResult> UpdatePlatform(int platformId, [FromForm] CreatePlatformDTO createPlatformDTO)
        {
            var result = await _platformService.UpdatePlatform(platformId, createPlatformDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [Authorize(Roles = "STAFF, ADMIN")]
        [HttpDelete]
        public async Task<IActionResult> RemovePlatform(int platformId)
        {
            var result = await _platformService.RemovePlatform(platformId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "STAFF, CUSTOMER")]
        [HttpPost("project/add")]
        public async Task<IActionResult> CreateProjectPlatform([FromForm]ProjectPlatformDTO projectPlatformDTO)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            //var check = await _authenService.CheckIfUserHasPermissionsToUpdateOrDeleteByProjectId(projectPlatformDTO.ProjectId, user);
            //if (check != null)
            //{
            //    return check;
            //}
            var result = await _platformService.CreateProjectPlatform(projectPlatformDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "STAFF, CUSTOMER")]
        [HttpDelete("project/delete")]
        public async Task<IActionResult> RemoveProjectPlatform(ProjectPlatformDTO projectPlatformDTO)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            //var check = await _authenService.CheckIfUserHasPermissionsToUpdateOrDeleteByProjectId(projectPlatformDTO.ProjectId, user);
            //if (check != null)
            //{
            //    return check;
            //}
            var result = await _platformService.RemoveProjectPlatform(projectPlatformDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }

}

