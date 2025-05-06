using Application.IService;
using Application.ViewModels.RewardDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CapstonProjectBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardController : ControllerBase
    {
        private readonly IRewardService _rewardService;
        private readonly IAuthenService _authenService;
        public RewardController(IRewardService rewardService, IAuthenService authenService)
        {
            _rewardService = rewardService;
            _authenService = authenService;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("GetAllReward")]
        public async Task<IActionResult> GetAllReward()
        {
            var result = await _rewardService.GetAllReward();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("GetRewardById")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRewardById(int rewardId)
        {
            var result = await _rewardService.GetRewardById(rewardId);
            if (result.Success && result.Data != null)
            {
                var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
                var check = await _authenService.CheckIfUserCanGetByProjectId(result.Data.ProjectId, user);
                if (check != null)
                {
                    return check;
                }
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("GetRewardByProjectId")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRewardByProjectId(int projectId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _authenService.CheckIfUserCanGetByProjectId(projectId, user);
            if (check != null)
            {
                return check;
            }
            var result = await _rewardService.GetRewardsByProjectId(projectId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize(Roles = "CUSTOMER")]
        [HttpPost("AddReward")]
        public async Task<IActionResult> AddReward(AddReward reward)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _authenService.CheckIfUserHasPermissionsToUpdateOrDeleteByProjectId(reward.ProjectId, user);
            if (check != null)
            {
                return check;
            }
            var result = await _rewardService.AddReward(reward);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize(Roles = "CUSTOMER")]
        [HttpPut("UpdateReward")]
        public async Task<IActionResult> UpdateReward(int rewardId, [FromForm] UpdateReward updateReward)
        {
            var reward = await _rewardService.GetRewardById(rewardId);
            if (reward.Success && reward.Data != null)
            {
                var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
                var check = await _authenService.CheckIfUserHasPermissionsToUpdateOrDeleteByProjectId(reward.Data.ProjectId, user);
                if (check != null)
                {
                    return check;
                }
                var result = await _rewardService.UpdateReward(rewardId, updateReward);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest(reward);
        }

        [Authorize]
        [HttpDelete("DeleteReward")]
        public async Task<IActionResult> DeleteReward(int rewardId)
        {
            var reward = await _rewardService.GetRewardById(rewardId);
            if (reward.Success && reward.Data != null)
            {
                var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
                var check = await _authenService.CheckIfUserHasPermissionsToUpdateOrDeleteByProjectId(reward.Data.ProjectId, user);
                if (check != null)
                {
                    return check;
                }
                var result = await _rewardService.DeleteReward(rewardId);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest(reward);
        }
    }
}
