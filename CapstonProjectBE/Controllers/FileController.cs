using Application.IService;
using Application.ViewModels.FileDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapstonProjectBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class FileController : Controller
    {
        private readonly IFileService _fileService;
        private readonly IAuthenService _authenService;
        public FileController(IFileService fileService, IAuthenService authenService)
        {
            _fileService = fileService;
            _authenService = authenService;
        }

        [Authorize(Roles = "CUSTOMER")]
        [HttpPost]
        public async Task<IActionResult> CreateFiles([FromForm] List<IFormFile> formFiles)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _fileService.CreateFiles(user.UserId, formFiles);
            if (!result.Success)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet("file/{fileId}")]
        public async Task<IActionResult> GetFileById([FromRoute] int fileId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            return await _fileService.GetFileById(fileId, user);
        }

        [Authorize]
        [HttpGet("files/user/{userId}")]
        public async Task<IActionResult> GetFilesByUserId([FromRoute] int userId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            return await _fileService.GetFilesByUserId(userId);
        }

        [Authorize]
        [HttpGet("files/user/paging/{userId}")]
        public async Task<IActionResult> GetPaginatedFilesByUserId([FromRoute] int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            return await _fileService.GetPaginatedFilesByUserId(userId, page, pageSize, user);
        }

        [Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdatePost([FromBody] UpdateFileDTO updateFileDTO)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _fileService.CheckIfUserHasPermissionsByFileId(updateFileDTO.FileId, user);
            if (check != null)
            {
                return check;
            }
            var result = await _fileService.UpdateFile(updateFileDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpDelete("Remove")]
        public async Task<IActionResult> RemoveFile(int fileId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _fileService.CheckIfUserHasPermissionsByFileId(fileId, user);
            if (check != null)
            {
                return check;
            }
            var result = await _fileService.RemoveFile(fileId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpDelete("softremove")]
        public async Task<IActionResult> SoftRemoveFile(int fileId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            var check = await _fileService.CheckIfUserHasPermissionsByFileId(fileId, user);
            if (check != null)
            {
                return check;
            }
            var result = await _fileService.SoftRemoveFile(fileId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


    }
}
