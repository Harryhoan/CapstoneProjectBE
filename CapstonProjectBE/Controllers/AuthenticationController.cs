using Application.IService;
using Application.ViewModels.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CapstonProjectBE.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenService _authenService;

        /// <summary>
        /// Constructor for the AuthenticationController.
        /// </summary>
        /// <param name="authent">The authentication service used to handle authentication requests.</param>
        public AuthenticationController(IAuthenService authent)
        {
            _authenService = authent;
        }

        /// <summary>
        /// Registers a new user with the provided registration details.
        /// </summary>
        /// <param name="registerObject">The registration details for the new user.</param>
        /// <returns>A response indicating success or failure of the registration.</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDTO registerObject)
        {
            var result = await _authenService.RegisterAsync(registerObject);

            if (!result.Success)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }

        /// <summary>
        /// Logs in an existing user with the provided login details.
        /// </summary>
        /// <param name="loginObject">The login details including username and password.</param>
        /// <returns>A response indicating success or failure of the login, along with a token and user role if successful.</returns>
        [HttpPost("login")]
        [AllowAnonymous]

        public async Task<IActionResult> LoginAsync(LoginUserDTO loginObject)
        {
            var result = await _authenService.LoginAsync(loginObject);

            if (!result.Success)
            {
                return StatusCode(401, result);
            }
            else
            {
                return Ok(
                    new
                    {
                        success = result.Success,
                        message = result.Message,
                        token = result.DataToken,
                        role = result.Role,
                        avatar = result.Avatar,
                        fullName = result.FullName,
                        hint = result.HintId,
                    }
                );
            }
        }
        /// <summary>
        /// Resends the confirmation token to the specified email address.
        /// </summary>
        /// <param name="sEmail">The email address to which the confirmation token will be resent.</param>
        /// <returns>A response indicating success or failure of the resend action.</returns>
        [HttpPost("resend")]
        //[Authorize(Roles = "Customer")]
        public async Task<IActionResult> ReSendConfirm(string sEmail)
        {
            var result = await _authenService.ResendConfirmationTokenAsync(sEmail);

            if (!result.Success)
            {
                return StatusCode(401, result);
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpPost("CreateStaff")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateStaffAccount(RegisterDTO register)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);

            if (user == null) return Unauthorized();

            var result = await _authenService.CreateStaffAccountAsync(user.UserId, register);

            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }
        [HttpPost("ForgetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var result = await _authenService.ForgetPasswordAsync(email);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }
        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string token, string newPassword)
        {
            var result = await _authenService.ResetPasswordAsync(token, newPassword);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpGet("CheckIfUserHasPermissionsToUpdateOrDeleteByProjectId")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckIfUserHasPermissionsToUpdateOrDeleteByProjectId(int projectId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _authenService.CheckIfUserHasPermissionsToUpdateOrDeleteByProjectId(projectId, user);
            if (check != null)
            {
                return check;
            }
            return Ok();
        }

        [HttpGet("CheckIfUserCanGetByProjectId")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckIfUserCanGetByProjectId(int projectId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _authenService.CheckIfUserCanGetByProjectId(projectId, user);
            if (check != null)
            {
                return check;
            }
            return Ok();
        }


    }
}
