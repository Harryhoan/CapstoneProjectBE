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

        public async Task<ServiceResponse<List<UserDTO>>> GetAllCustomerAsync()
        {
            var response = new ServiceResponse<List<UserDTO>>();
            try
            {
                var customer = await _unitOfWork.UserRepo.GetAllAsync();
                var data = _mapper.Map<List<UserDTO>>(customer.Where(u => u.Role == UserEnum.CUSTOMER));

                response.Data = data;
                response.Success = true;
                response.Message = "Get all Customer successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to Get all Customer: {ex.Message}";
            }
            return response;
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
                response.Message = "Get user successfully";
                response.Success = true;
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
                if (user.IsDeleted)
                {
                    response.Success = false;
                    response.Message = "User has been deleted.";
                    return response;
                }
                response.Message = "Get user successfully";
                response.Success = true;
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
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }
                var users = await _userRepo.GetAllUser();
                if (user.Role == UserEnum.STAFF)
                {
                    var staffUsers = _mapper.Map<IEnumerable<UserDTO>>(users.Where(u => u.Role == UserEnum.CUSTOMER && !u.IsDeleted));
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
                    userEntity.Fullname = FormatUtils.TrimSpacesPreserveSingle(userEntity.Fullname);
                }
                if (!string.IsNullOrWhiteSpace(UpdateUser.Email))
                {
                    userEntity.Email = UpdateUser.Email;
                }
                if (!string.IsNullOrWhiteSpace(UpdateUser.Password))
                {
                    userEntity.Password = HashPassWithSHA256.HashWithSHA256(UpdateUser.Password);
                }
                if (!string.IsNullOrEmpty(UpdateUser.PaymentAccount))
                {
                    userEntity.PaymentAccount = UpdateUser.PaymentAccount;
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

                userEntity.Avatar = uploadResult.SecureUrl.ToString();
                await _unitOfWork.UserRepo.UpdateAsync(userEntity);
                response.Data = _mapper.Map<string>(userEntity);
                response.Message = "Get user successfully";
                response.Success = true;
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
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }
                if (user.Role != UserEnum.ADMIN)
                {
                    response.Success = false;
                    response.Message = "You are not allowed to do this.";
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

        public async Task<ServiceResponse<UserDTO>> GetUserByEmailAsync(string email)
        {
            var response = new ServiceResponse<UserDTO>();
            try
            {
                var user = await _unitOfWork.UserRepo.FindEntityAsync(u => u.Email.Equals(email));
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
                response.Message = "Get user successfully";
                response.Success = true;
                response.Data = _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<UserDTO>> UpdatePasswordUser(string email, string password, int id)
        {
            var response = new ServiceResponse<UserDTO>();
            try
            {
                var hashedPassword = HashPassWithSHA256.HashWithSHA256(password);
                var checkCode = await _unitOfWork.VerifyCodeRepo.FindEntityAsync(c => c.Email.Equals(email));
                var user = await _unitOfWork.UserRepo.FindEntityAsync(u => u.Email.Equals(email) && u.UserId.Equals(id));

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }
                if (checkCode == null || checkCode.IsVerified == false)
                {
                    response.Success = false;
                    response.Message = "Verification code not found or not verified";
                    return response;
                }

                user.Password = hashedPassword;
                await _unitOfWork.UserRepo.UpdateAsync(user);
                await _unitOfWork.VerifyCodeRepo.RemoveAsync(checkCode);

                response.Data = _mapper.Map<UserDTO>(user);
                response.Success = true;
                response.Message = "Password updated successfully";
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
