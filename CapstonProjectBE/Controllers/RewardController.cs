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

        [HttpGet("GetRewardById")]
        public async Task<IActionResult> GetRewardById(int rewardId)
        {
            return Ok(await _rewardService.GetRewardById(rewardId));
        }

        [HttpGet("GetRewardByProjectId")]
        public async Task<IActionResult> GetRewardByProjectId(int projectId)
        {
            return Ok(await _rewardService.GetRewardByProjectId(projectId));
        }

        [HttpPost("AddReward")]
        public async Task<IActionResult> AddReward(AddReward reward)
        {
            var newReward = await _rewardService.AddReward(reward);
            return Ok(newReward);
        }

        [HttpPut("UpdateReward")]
        public async Task<IActionResult> UpdateReward(int rewardId, [FromForm] UpdateReward updateReward)
        {
            var updatedReward = await _rewardService.UpdateReward(rewardId, updateReward);
            return Ok(updatedReward);
        }

        [HttpDelete("DeleteReward")]
        public async Task<IActionResult> DeleteReward(int rewardId)
        {
            var response = await _rewardService.DeleteReward(rewardId);
            return Ok(response);
        }
    }
}
