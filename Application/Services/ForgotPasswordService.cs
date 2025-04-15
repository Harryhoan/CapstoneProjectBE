using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels.UserDTO;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ForgotPasswordService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<VerifyCode>> AddCode(string code, string email)
        {
            var response = new ServiceResponse<VerifyCode>();
            try
            {
                var verifyCode = new VerifyCode
                {
                    Email = email,
                    Code = code,
                    CreateAt = DateTime.UtcNow,
                };
                await _unitOfWork.VerifyCodeRepo.AddAsync(verifyCode);
                response.Data = verifyCode;
                response.Success = true;
                response.Message = "Code added successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> Delete(string email)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var verifyCode = await _unitOfWork.VerifyCodeRepo.FindEntityAsync(c => c.Email.Equals(email));
                if (verifyCode == null)
                {
                    response.Success = false;
                    response.Message = "Email not found";
                    return response;
                }

                await _unitOfWork.VerifyCodeRepo.RemoveAsync(verifyCode);

                response.Data = true;
                response.Success = true;
                response.Message = "Delete successful";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> IsCodeExist(string email)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var exists = await _unitOfWork.VerifyCodeRepo.FindEntityAsync(vc => vc.Email.Equals(email));

                response.Data = exists != null;
                response.Success = true;
                response.Message = exists != null ? "Code exists" : "Code does not exist";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return response;
        }

        public async Task<ServiceResponse<UserDTO>> VerifyCode(string code, string email)
        {
            var response = new ServiceResponse<UserDTO>();
            try
            {
                var user = await _unitOfWork.UserRepo.FindEntityAsync(u => u.Email.Equals(email));
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Email is invalid";
                    return response;
                }

                var verifyCode = await _unitOfWork.VerifyCodeRepo.FindEntityAsync(vc => vc.Code.Equals(code) && vc.Email.Equals(email));
                if (verifyCode == null)
                {
                    response.Success = false;
                    response.Message ="Verify code incorrect";
                    return response;
                } 

                var createAt = verifyCode.CreateAt;
                DateTime now = DateTime.UtcNow;
                TimeSpan timeSpan = now - createAt;
                if (timeSpan.TotalSeconds > 60)
                {   
                    response.Success = false;
                    response.Message = "Verify code expired";
                    return response;
                }


                response.Data = _mapper.Map<UserDTO>(user);
                response.Success = true;
                response.Message = "Verify Code successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return response;
        }
    }
}
