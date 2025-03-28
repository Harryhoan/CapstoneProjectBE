using Application.IRepositories;
using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels.UserDTO;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper, Cloudinary cloudinary, IUserRepo userRepo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinary = cloudinary;
            _userRepo = userRepo;
        }

        public async Task<ServiceResponse<UserDTO>> GetUserByIdAsync(int userId)
        {
            var response = new ServiceResponse<UserDTO>();
            try
            {
                var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }
                response.Data = _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ServiceResponse<UserDTO>> GetUserByUserIdByMonitorAsync(int userId)
        {
            var response = new ServiceResponse<UserDTO>();
            try
            {
                var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }
                response.Data = _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ServiceResponse<IEnumerable<UserDTO>>> GetAllUserAsync()
        {
            var response = new ServiceResponse<IEnumerable<UserDTO>>();
            try
            {
                var users = await _userRepo.GetAllUser();
                response.Data = _mapper.Map<IEnumerable<UserDTO>>(users);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ServiceResponse<UpdateUserDTO>> UpdateUserAsync(UpdateUserDTO UpdateUser, int userId)
        {
            var response = new ServiceResponse<UpdateUserDTO>();
            try
            {
                var userEntity = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                if (userEntity == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }
                if (!string.IsNullOrEmpty(UpdateUser.Fullname))
                {
                    userEntity.Fullname = UpdateUser.Fullname;
                }
                if (!string.IsNullOrEmpty(UpdateUser.Email))
                {
                    userEntity.Email = UpdateUser.Email;
                }
                if (!string.IsNullOrEmpty(UpdateUser.Password))
                {
                    userEntity.Password = UpdateUser.Password;
                }
                if (!string.IsNullOrEmpty(UpdateUser.Phone))
                {
                    userEntity.Phone = UpdateUser.Phone;
                }
                if (!string.IsNullOrEmpty(UpdateUser.Bio))
                {
                    userEntity.Bio = UpdateUser.Bio;
                }
                await _unitOfWork.UserRepo.UpdateAsync(userEntity);

                response.Success = true;
                response.Message = "Update User Successfully";
                response.Data = _mapper.Map<UpdateUserDTO>(userEntity);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ServiceResponse<string>> UpdateUserAvatarAsync(int userId, IFormFile avatarFile)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var userEntity = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                if (userEntity == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }

                var uploadResult = new ImageUploadResult();
                if (avatarFile.Length > 0)
                {
                    using (var stream = avatarFile.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(avatarFile.FileName, stream)
                        };
                        uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    }
                }

                userEntity.Avatar = uploadResult.Url.ToString();
                await _unitOfWork.UserRepo.UpdateAsync(userEntity);
                response.Data = _mapper.Map<string>(userEntity);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
