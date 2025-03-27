using Application.IService;
using Application.ViewModels.UserDTO;
using AutoMapper;
using Domain;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Infrastructure;
using Microsoft.Extensions.Options;
using Application.ViewModels;
using Application.Services;

namespace CapstonProjectBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;
        private readonly IUserService _userService;
        private readonly IAuthenService _authenService;
        private readonly IMapper _mapper;
        private readonly ApiContext _context;

        public UserController(IOptions<Cloud> config, IUserService userService, IMapper mapper, ApiContext context, IAuthenService authenService)
        {
            var cloudinaryAccount = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(cloudinaryAccount);
            _userService = userService;
            _mapper = mapper;
            _context = context;
            _authenService = authenService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            var response = await _userService.GetAllUserAsync();
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        /// <summary>
        /// Retrieves user information based on the authenticated user's token. Access is restricted to users with
        /// 'Customer' or 'Admin' roles.
        /// </summary>
        /// <returns>Returns an IActionResult indicating the success or failure of the user retrieval operation.</returns>
        [Authorize(Roles = "Customer")]
        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById()
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var response = await _userService.GetUserByIdAsync(user.UserId);
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        /// <summary>
        /// Retrieves user information based on a specified identifier for authorized staff or admin users.
        /// </summary>
        /// <param name="userId">The identifier used to look up the user in the system.</param>
        /// <returns>Returns an action result indicating the success or failure of the user retrieval operation.</returns>
        [Authorize(Roles = "Staff, Admin")]
        [HttpGet("GetUserByUserId")]
        public async Task<IActionResult> GetUserByUserIdByMonitor(int userId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var response = await _userService.GetUserByIdAsync(userId);
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        /// <summary>
        /// Handles the update of user information based on the provided data and the authenticated user's context.
        /// </summary>
        /// <param name="UpdateUser">Contains the new user information to be updated in the system.</param>
        /// <returns>Returns an action result indicating the success or failure of the update operation.</returns>
        [Authorize(Roles = "Customer, Staff, Admin")]
        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO UpdateUser)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var response = await _userService.UpdateUserAsync(UpdateUser, user.UserId);
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [Authorize(Roles = "Customer")]
        [HttpPut("avatar")]
        public async Task<IActionResult> UpdateUserAvatar(IFormFile file)
        {
            var AuthorizeUser = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (AuthorizeUser == null)
            {
                return Unauthorized();
            }
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Crop("fill").Gravity("face")
                    };
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
            }

            if (uploadResult.Url == null)
                return BadRequest("Could not upload image");

            // Update the image URL in the database
            AuthorizeUser.Avatar = uploadResult.Url.ToString();
            await _context.SaveChangesAsync();

            return Ok(new { imageUrl = AuthorizeUser.Avatar });
        }
    }
}
