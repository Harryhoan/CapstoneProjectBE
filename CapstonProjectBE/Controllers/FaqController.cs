using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels.FaqDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CapstonProjectBE.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class FaqController : ControllerBase
    {
        private readonly IFAQService _faqService;
        private readonly IAuthenService _authenService;
        public FaqController(IFAQService faqService, IAuthenService authenService)
        {
            _faqService = faqService;
            _authenService = authenService;
        }

        [HttpGet("GetFaqByProjectId")]
        [AllowAnonymous]
        public async Task<ServiceResponse<List<ViewFaqDto>>> GetAllFaqByProjectIdAsync(int projectId)
        {
            return await _faqService.GetAllFaqByProjectIdAsync(projectId);
        }

        [HttpGet("GetFaqProjectId")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetFaqByProjectId(int projectId)
        {
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
            var result = await _faqService.GetFaqByProjectId(user.UserId, projectId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddFaq")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> AddFaq(int projectId, [FromForm] FaqDto createFaq)
        {
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
            var result = await _faqService.AddFaq(user.UserId, projectId, createFaq);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPut("UpdateFaq")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> UpdateFaq(int projectId, string oldQuestion, [FromForm] FaqDto updateFaq)
        {
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
            var result = await _faqService.UpdateFaq(user.UserId, projectId, oldQuestion, updateFaq);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpDelete("DeleteFaq")]
        [Authorize(Roles = "CUSTOMER, STAFF")]
        public async Task<IActionResult> DeleteFaq(int projectId, string question)
        {
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
            var result = await _faqService.DeleteFAQ(user.UserId, projectId, question);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
