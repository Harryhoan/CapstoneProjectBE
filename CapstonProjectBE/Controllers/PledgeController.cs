using Application;
using Application.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapstonProjectBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PledgeController : ControllerBase
    {
        private readonly IPledgeService _pledgeService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenService _authenService;
        public PledgeController(IPledgeService pledgeService, IAuthenService authenService, IUnitOfWork unitOfWork)
        {
            _pledgeService = pledgeService;
            _authenService = authenService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetAllPledges")]
        [Authorize(Roles = "ADMIN, STAFF")]
        public async Task<IActionResult> GetAllPledgeByAdmin()
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _pledgeService.GetAllPledgeByAdmin();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("GetPledgeById")]
        [Authorize(Roles = "ADMIN, STAFF, CUSTOMER")]
        public async Task<IActionResult> GetPledgeById(int pledgeId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _pledgeService.GetPledgeById(pledgeId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("GetPledgeByUserId")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetPledgeByUserId()
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _pledgeService.GetPledgeByUserId(user.UserId);
            if (!result.Success)
            {
                return BadRequest();
            }
            return Ok(result);
        }
        [HttpGet("GetBacker/{projectId}")]
        [Authorize]
        public async Task<IActionResult> GetBackerByProjectId([FromRoute] int projectId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _authenService.CheckIfUserHasPermissionsToUpdateOrDeleteByProjectId(projectId, user);
            if (check != null)
            {
                return check;
            }
            var result = await _pledgeService.GetBackerByProjectId(projectId);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result);
        }
        [HttpGet("GetBackerByAdmin/{projectId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetBackerByProjectIdForAdmin([FromRoute] int projectId)
        {
            var result = await _pledgeService.GetBackerByProjectIdForAdmin(projectId);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result);
        }
        [HttpGet("ExportPledgesToExcel/{projectId}")]
        [Authorize(Roles = "CUSTOMER, STAFF, ADMIN")]
        public async Task<IActionResult> ExportPledgesToExcel(int projectId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _authenService.CheckIfUserHasPermissionsToUpdateOrDeleteByProjectId(projectId, user);
            if (check != null)
            {
                return check;
            }
            var result = await _pledgeService.ExportPledgeToExcelByProjectId(projectId);
            if (!result.Success || string.IsNullOrEmpty(result.Data)) return BadRequest(result.Message);
            var project = await _unitOfWork.ProjectRepo.GetProjectById(projectId);
            if (project == null) return BadRequest("Project not found");
            var fileBytes = Convert.FromBase64String(result.Data);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Project_{project.Title}.xlsx");
        }
    }
}
