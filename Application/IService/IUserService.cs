using Application.ServiceResponse;
using Application.ViewModels.UserDTO;
using Microsoft.AspNetCore.Http;

namespace Application.IService
{
    public interface IUserService
    {
        public Task<ServiceResponse<UserDTO>> GetUserByIdAsync(int userId);
        public Task<ServiceResponse<IEnumerable<UserDTO>>> GetAllUserAsync(int userId);
        public Task<ServiceResponse<UpdateUserDTO>> UpdateUserAsync(UpdateUserDTO UpdateUser, int userId);
        Task<ServiceResponse<string>> UpdateUserAvatarAsync(int userId, IFormFile avatarFile);
        public Task<ServiceResponse<UserDTO>> GetUserByUserIdByMonitorAsync(int userId);
        public Task<ServiceResponse<string>> DeleteUserAsync(int userId, int UserDeleteId);
    }
}
