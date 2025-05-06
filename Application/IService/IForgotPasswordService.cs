using Application.ServiceResponse;
using Application.ViewModels.UserDTO;
using Domain.Entities;

namespace Application.IService
{
    public interface IForgotPasswordService
    {
        public Task<ServiceResponse<VerifyCode>> AddCode(string code, string email);
        public Task<ServiceResponse<UserDTO>> VerifyCode(string code, string email);
        public Task<ServiceResponse<bool>> IsCodeExist(string email);
        public Task<ServiceResponse<bool>> Delete(string email);
    }
}
