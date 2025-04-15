using Application.ServiceResponse;
using Application.ViewModels.UserDTO;
using Microsoft.AspNetCore.Http;

namespace Application.IService
{
    public interface IUserService
    {
        public Task<ServiceResponse<UserDTO>> GetUserByIdAsync(int userId);
        public Task<ServiceResponse<UserDTO>> GetUserByEmailAsync(string email);
        public Task<ServiceResponse<IEnumerable<UserDTO>>> GetAllUserAsync(int userId);
        public Task<ServiceResponse<UpdateUserDTO>> UpdateUserAsync(UpdateUserDTO UpdateUser, int userId);
        public Task<ServiceResponse<UserDTO>> UpdatePasswordUser(string email, string password, int id);
        Task<ServiceResponse<string>> UpdateUserAvatarAsync(int userId, IFormFile avatarFile);
        public Task<ServiceResponse<UserDTO>> GetUserByUserIdByMonitorAsync(int userId);
        public Task<ServiceResponse<string>> DeleteUserAsync(int userId, int UserDeleteId);
    }
}
