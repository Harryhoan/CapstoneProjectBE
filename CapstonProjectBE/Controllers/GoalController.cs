using Application.IService;
using Application.ViewModels.GoalDTO;
using Application.ViewModels.RewardDTO;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CapstonProjectBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoalController : ControllerBase
    {
        private readonly IGoalService _goalService;
        public GoalController(IGoalService goalService)
        {
            _goalService = goalService;
        }

        [HttpGet("GetAllGoal")]
        public async Task<IActionResult> GetAllGoal()
        {
            return Ok(await _goalService.GetAllGoal());
        }

        [HttpPost("AddGoal")]
        public async Task<IActionResult> AddGoal(int projectId, CreateGoal createGoal)
        {
            var newGoal = await _goalService.AddGoal(projectId, createGoal);
            return Ok(newGoal);
        }
    }
}
