using Application.IService;
using Application.ViewModels.FaqDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CapstonProjectBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaqController : ControllerBase
    {
        private readonly IFAQService _faqService;
        private readonly IAuthenService _authenService;
        public FaqController(IFAQService faqService)
        {
            _faqService = faqService;
        }

        [HttpGet("GetFaqProjectId")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetFaqByProjectId(int projectId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(await _faqService.GetFaqByProjectId(user.UserId, projectId));
        }
        [HttpPost("AddFaq")]
        public async Task<IActionResult> AddFaq(int projectId, FaqDto createFaq)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(await _faqService.AddFaq(user.UserId, projectId, createFaq));
        }
        [HttpPut("UpdateFaq")]
        public async Task<IActionResult> UpdateFaq(int projectId, string question, FaqDto updateFaq)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(await _faqService.UpdateFaq(user.UserId, projectId, question, updateFaq));
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteFaq(int projectId, string question)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(await _faqService.DeleteFAQ(user.UserId, projectId, question));
        }
    }
}
