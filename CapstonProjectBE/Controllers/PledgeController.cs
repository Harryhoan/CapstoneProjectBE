using Application.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CapstonProjectBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PledgeController : ControllerBase
    {
        private readonly IPledgeService _pledgeService;
        private readonly IAuthenService _authenService;
        public PledgeController(IPledgeService pledgeService, IAuthenService authenService)
        {
            _pledgeService = pledgeService;
            _authenService = authenService;
        }

        [HttpGet("GetAllPledges")]
        [Authorize(Roles = "Admin, Staff")]
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
        [Authorize(Roles = "Admin, Customer, Staff")]
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
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetPledgeByUserId()
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if(user == null)
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
    }
}
