using Application.IService;
using Application.ViewModels.PlatformDTO;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CapstonProjectBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformService _platformService;
        public PlatformController(IPlatformService platformService)
        {
            _platformService = platformService;
        }
        [HttpPost("CreatePlatform")]
        public async Task<IActionResult> CreatePlatform(int userId, CreatePlatformDto createPlatformDto)
        {
            var platform = await _platformService.CreatePlatform(userId, createPlatformDto);
            return Ok(platform);
        }
    }
}
