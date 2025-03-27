using Application.ServiceResponse;
using Application.ViewModels.UserDTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IAuthenService
    {
        public Task<ServiceResponse<RegisterDTO>> RegisterAsync(RegisterDTO userObject);
        public Task<TokenResponse<string>> LoginAsync(LoginUserDTO userObject);
        public Task<User?> GetUserByTokenAsync(ClaimsPrincipal claims);
        public Task<ServiceResponse<string>> ResendConfirmationTokenAsync(string email);
    }
}
