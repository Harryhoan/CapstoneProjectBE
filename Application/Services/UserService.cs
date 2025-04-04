using Application.IRepositories;
using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels.UserDTO;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

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
                if (user.IsDeleted)
                {
                    response.Success = false;
                    response.Message = "User has been deleted.";
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
        public async Task<ServiceResponse<IEnumerable<UserDTO>>> GetAllUserAsync(int userId)
        {
            var response = new ServiceResponse<IEnumerable<UserDTO>>();
            try
            {
                var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                var users = await _userRepo.GetAllUser();
                if (user.Role == UserEnum.STAFF)
                {
                    var staffUsers = _mapper.Map<IEnumerable<UserDTO>>(users.Where(u => u.Role == UserEnum.CUSTOMER));
                    response.Data = staffUsers;
                    response.Success = true;
                    response.Message = "Get all user successfully";
                    return response;
                }
                response.Data = _mapper.Map<IEnumerable<UserDTO>>(users);
                response.Success = true;
                response.Message = "Get all user successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
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
                    userEntity.Password = HashPassWithSHA256.HashWithSHA256(UpdateUser.Password);
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
        public async Task<ServiceResponse<string>> DeleteUserAsync(int userId, int UserDeleteId)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                if (user.Role != UserEnum.ADMIN)
                {
                    response.Success = false;
                    response.Message = "You are not allow to do this.";
                    return response;
                }
                var DeleteUser = await _unitOfWork.UserRepo.GetByIdAsync(UserDeleteId);
                if (DeleteUser == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                DeleteUser.IsDeleted = true;
                await _unitOfWork.UserRepo.UpdateAsync(DeleteUser);

                response.Success = true;
                response.Message = "Delete User Successfully";
                return response;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to delete user: {ex.Message}";
                return response;
            }
        }
    }
}
