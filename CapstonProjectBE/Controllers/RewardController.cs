using Application.IRepositories;
using Application.IService;
using Application.ViewModels.RewardDTO;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CapstonProjectBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardController : ControllerBase
    {
        private readonly IRewardService _rewardService;
        public RewardController(IRewardService rewardService)
        {
            _rewardService = rewardService;
        }

        [HttpGet("GetAllReward")]
        public async Task<IActionResult> GetAllReward()
        {
            return Ok(await _rewardService.GetAllReward());
        }

        [HttpPost("AddReward")]
        public async Task<IActionResult> AddReward(AddReward reward)
        {
            var newReward = await _rewardService.AddReward(reward);
            return Ok(newReward);
        }
    }
}
