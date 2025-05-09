using Application;
using Application.Commons;
using Application.IService;
using Application.Utils;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CapstonProjectBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly IForgotPasswordService _forgotPasswordService;
        private readonly IUserService _userService;
        private readonly IAuthenService _authenService;
        private readonly AppConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        public ForgotPasswordController(IForgotPasswordService forgotPasswordService, IAuthenService authenService, IUserService userService, AppConfiguration appConfiguration, IUnitOfWork unitOfWork)
        {
            _authenService = authenService;
            _forgotPasswordService = forgotPasswordService;
            _userService = userService;
            _config = appConfiguration;
            _unitOfWork = unitOfWork;
        }
        [HttpGet("Verify-Code")]
        public async Task<IActionResult> VerifyCode([FromQuery] string code, [FromQuery] string email)
        {
            var user = await _forgotPasswordService.VerifyCode(code, email);
            return Ok(user);
        }

        [HttpPost("Send-Code")]
        public async Task<IActionResult> CheckEmailAndSendCode([FromForm] string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user != null)
            {
                var codeExist = await _forgotPasswordService.IsCodeExist(email);
                if (codeExist.Data)
                {
                    await _forgotPasswordService.Delete(email);
                }
                string code = CodeGenerator.GenerateRandomVerifyCode();
                await _forgotPasswordService.AddCode(code, email);

                var emailSent = await EmailSender.SendCode(email, code);
                if (!emailSent)
                {
                    return BadRequest("Failed to send password reset email");
                }
                var result = new Dictionary<string, string>() {
                    {"email",email }
                };
                return Ok(result);
            }
            else
            {
                return BadRequest("User not found!");
            }

        }

        [HttpPost("Reset-Password")]
        public async Task<IActionResult> ResetPassword(string email, string newPassword)
        {
            var check = await _userService.GetUserByEmailAsync(email);
            if (check.Data == null)
            {
                return BadRequest("User not found");
            }
            var user = await _userService.UpdatePasswordUser(email, newPassword, check.Data.UserId);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return BadRequest("Cannot reset password!");
            }
        }
    }
}
