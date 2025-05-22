using Application.IRepositories;
using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels.PostDTO;
using Application.ViewModels.UserDTO;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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

        public async Task<ServiceResponse<UserDTO>> UpdateUserAsync(UpdateUserDTO updateUser, int userId)
        {
            var response = new ServiceResponse<UserDTO>();
            try
            {
                var validationContext = new ValidationContext(updateUser);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(updateUser, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                var userEntity = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                if (userEntity == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }
                if (!string.IsNullOrEmpty(updateUser.Fullname))
                {
                    userEntity.Fullname = FormatUtils.TrimSpacesPreserveSingle(userEntity.Fullname);
                }
                if (!string.IsNullOrWhiteSpace(updateUser.Email))
                {
                    userEntity.Email = updateUser.Email;
                }
                if (!string.IsNullOrWhiteSpace(updateUser.Password))
                {
                    userEntity.Password = HashPassWithSHA256.HashWithSHA256(updateUser.Password);
                }
                //if (!string.IsNullOrWhiteSpace(updateUser.PaymentAccount))
                //{
                //    if (string.IsNullOrWhiteSpace(userEntity.PaymentAccount))
                //    {
                //        userEntity.PaymentAccount = updateUser.PaymentAccount.Trim();
                //    }
                //    else if (!userEntity.PaymentAccount.Trim().ToLower().Equals(updateUser.PaymentAccount.Trim().ToLower(), StringComparison.OrdinalIgnoreCase) && !(await _unitOfWork.ProjectRepo.Any(p => p.TransactionStatus == TransactionStatusEnum.RECEIVING) && !(await _unitOfWork.PledgeDetailRepo.Any(p => p.Pledge.UserId == userEntity.UserId && (p.Status == PledgeDetailEnum.REFUNDING || p.Status == PledgeDetailEnum.TRANSFERRING)))))
                //    {
                //        userEntity.PaymentAccount = updateUser.PaymentAccount.Trim();
                //    }
                //}
                //if (!string.IsNullOrEmpty(updateUser.Phone))
                //{
                //    userEntity.Phone = updateUser.Phone;
                //}
                if (updateUser.Bio != null)
                {
                    userEntity.Bio = FormatUtils.FormatText(updateUser.Bio.Trim());
                }
                await _unitOfWork.UserRepo.UpdateAsync(userEntity);

                response.Success = true;
                response.Message = "Update User Successfully";
                response.Data = _mapper.Map<UserDTO>(userEntity);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<UserDTO>> VerifyUserAsync(VerifyUserDTO verifyUserDTO, Domain.Entities.User user)
        {
            var response = new ServiceResponse<UserDTO>();
            try
            {
                var validationContext = new ValidationContext(verifyUserDTO);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(verifyUserDTO, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                if ((await _unitOfWork.ProjectRepo.Any(p => p.Status == ProjectStatusEnum.ONGOING) || await _unitOfWork.PledgeDetailRepo.Any(p => p.Pledge.UserId == user.UserId && (p.Status == PledgeDetailEnum.REFUNDING || p.Status == PledgeDetailEnum.TRANSFERRING))))
                {
                    response.Success = false;
                    response.Message = "Payment account and phone number cannot be changed when there are still ongoing projects or when transactions are currently being processed.";
                    return response;
                }

                var paymentAccount = string.IsNullOrWhiteSpace(verifyUserDTO.PaymentAccount) || !new EmailAddressAttribute().IsValid(verifyUserDTO.PaymentAccount) ? string.Empty : verifyUserDTO.PaymentAccount.Trim();
                var phone = string.IsNullOrWhiteSpace(verifyUserDTO.Phone) || !Regex.IsMatch(verifyUserDTO.Phone, @"^\d{10}$")  ? string.Empty : verifyUserDTO.Phone.Trim();

                user.PaymentAccount = paymentAccount;
                user.Phone = phone;
                if (string.IsNullOrWhiteSpace(user.Phone) || !new EmailAddressAttribute().IsValid(user.PaymentAccount) || string.IsNullOrWhiteSpace(user.PaymentAccount) || !Regex.IsMatch(user.Phone, @"^\d{10}$"))
                {
                    user.IsVerified = false;
                }
                else
                {
                    user.IsVerified = true;
                }
                await _userRepo.UpdateAsync(user);

                response.Success = true;
                response.Message = "Verify User Successfully";
                response.Data = _mapper.Map<UserDTO>(user);
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
                if (await _unitOfWork.ProjectRepo.Any(p => p.Status == ProjectStatusEnum.ONGOING && p.CreatorId == user.UserId))
                {
                    response.Success = false;
                    response.Message = "Campaginers whose projects are ongoing cannot be deleted.";
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
                var passwordPattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,15}$";
                if (!Regex.IsMatch(password, passwordPattern))
                {
                    response.Success = false;
                    response.Message = "Password must be 8–15 characters and include at least one uppercase letter, one lowercase letter, one digit, and one special character.";
                    return response;
                }

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
