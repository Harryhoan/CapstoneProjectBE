using Application.IService;
using Application.ViewModels.ReportDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapstonProjectBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IAuthenService _authenService;
        public ReportController(IReportService reportService, IAuthenService authenService)
        {
            _reportService = reportService;
            _authenService = authenService;
        }

        [HttpGet("GetAllReport")]
        [Authorize(Roles = "STAFF, ADMIN")]
        public async Task<IActionResult> GetAllReport()
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _reportService.GetAllReportAsync();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("GetReportByUserId")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetReportByUserId()
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _reportService.GetReportByUserIdAsync(user.UserId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("CreateReport")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CreateReport([FromBody] CreateReportDto report)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _reportService.CreateReportAsync(user.UserId, report);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("GetReportById")]
        [Authorize(Roles = "CUSTOMER, ADMIN, STAFF")]
        public async Task<IActionResult> GetReportById(int reportId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _reportService.GetReportByIdAsync(user.UserId, reportId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
