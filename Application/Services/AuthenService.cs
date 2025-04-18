using Application.Commons;
using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels.UserDTO;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Security.Claims;

namespace Application.Services
{
    public class AuthenService : IAuthenService
    {
        private readonly AppConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthenService(AppConfiguration config, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _config = config;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<RegisterDTO>> RegisterAsync(RegisterDTO userObject)
        {
            var response = new ServiceResponse<RegisterDTO>();
            try
            {
                //var existEmail = await _unitOfWork.UserRepo.CheckEmailAddressExisted(userObject.Email);
                var user = await _unitOfWork.UserRepo.GetByEmailAsync(userObject.Email);
                if (user != null && !user.IsDeleted)
                {
                    response.Success = false;
                    response.Message = "Email is already existed";
                    return response;
                }

                var userAccountRegister = _mapper.Map<User>(userObject);
                userAccountRegister.Password = HashPassWithSHA256.HashWithSHA256(userObject.Password);
                userAccountRegister.Role = UserEnum.CUSTOMER;
                userAccountRegister.CreatedDatetime = DateTime.UtcNow;
                await _unitOfWork.UserRepo.AddAsync(userAccountRegister);

                // Create Token
                var confirmationToken = new Token
                {
                    TokenValue = Guid.NewGuid().ToString(),
                    Type = "confirmation",
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                    UserId = userAccountRegister.UserId
                };
                await _unitOfWork.TokenRepo.AddAsync(confirmationToken);

                // Construct Confirmation Link
                var confirmationLink = $"https://marvelous-gentleness-production.up.railway.app/swagger/confirm?token={confirmationToken.TokenValue}";

                // Send Mail
                var emailSend = await EmailSender.SendConfirmationEmail(userObject.Email, confirmationLink);
                if (!emailSend)
                {
                    response.Success = false;
                    response.Message = "Error when sending mail";
                    return response;
                }

                var accountRegistedDTO = _mapper.Map<RegisterDTO>(userAccountRegister);
                response.Success = true;
                response.Data = accountRegistedDTO;
                response.Message = "Register successfully. Please check your email to confirm your account.";
            }
            catch (DbException e)
            {
                response.Success = false;
                response.Message = "Database error occurred.";
                response.ErrorMessages = new List<string> { e.Message };
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = "Error";
                response.ErrorMessages = new List<string> { e.Message };
            }

            return response;
        }

        public async Task<TokenResponse<string>> LoginAsync(LoginUserDTO userObject)
        {
            var response = new TokenResponse<string>();
            try
            {
                var passHash = HashPassWithSHA256.HashWithSHA256(userObject.Password);
                var userLogin = await _unitOfWork.UserRepo.GetUserByEmailAddressAndPasswordHash(userObject.Username, passHash);
                if (userLogin == null)
                {
                    response.Success = false;
                    response.Message = "Invalid username or password";
                    return response;
                }
                var token = await _unitOfWork.TokenRepo.GetTokenByUserIdAsync(userLogin.UserId);
                if (token != null && token.TokenValue != "success")
                {
                    response.Success = false;
                    response.Message = "Please confirm via link in your mail";
                    return response;
                }
                var auth = userLogin.Role;
                var userId = userLogin.UserId;
                var avatar = userLogin.Avatar;
                var fullName = userLogin.Fullname;
                var tokenJWT = userLogin.GenerateJsonWebToken(_config, _config.JWTSection.SecretKey, DateTime.Now);
                response.Success = true;
                response.Message = "Login successfully";
                response.DataToken = tokenJWT;
                response.Avatar = avatar;
                response.FullName = fullName;
                response.Role = auth.ToString();
                response.HintId = userId;
            }
            catch (DbException ex)
            {
                response.Success = false;
                response.Message = "Database error occurred.";
                response.ErrorMessages = new List<string> { ex.Message };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error";
                response.ErrorMessages = new List<string> { ex.Message };
            }

            return response;
        }

        public async Task<User?> GetUserByTokenAsync(ClaimsPrincipal claims)
        {
            if (claims == null)
            {
                return null;
            }
            var userId = claims.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
            {
                return null;
            }

            var user = await _unitOfWork.UserRepo.GetByIdAsync(id);
            return user;
        }
        public async Task<ServiceResponse<string>> ResendConfirmationTokenAsync(string email)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var user = await _unitOfWork.UserRepo.GetByEmailAsync(email);
                if (user == null)
                {
                    response.Success = false;
                    response.Error = "User not found.";
                    return response;
                }
                var token = await _unitOfWork.TokenRepo.FindByConditionAsync(user.UserId, "confirmation");
                if (token == null)
                {
                    response.Success = false;
                    response.Message = "No token found for this user";
                    return response;
                }
                if (token.TokenValue == "success")
                {
                    response.Success = false;
                    response.Message = "Your account has been confirmed.";
                    return response;
                }
                if (DateTime.UtcNow > token.ExpiresAt)
                {
                    await _unitOfWork.TokenRepo.RemoveAsync(token);
                    var newToken = new Token
                    {
                        TokenValue = Guid.NewGuid().ToString(),
                        Type = "confirmation",
                        CreatedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                        UserId = user.UserId
                    };

                    await _unitOfWork.TokenRepo.AddAsync(newToken);

                    var confirmationLink = $"https://marvelous-gentleness-production.up.railway.app/swagger/confirm?token={newToken.TokenValue}";
                    var emailSend = await EmailSender.SendConfirmationEmail(user.Email, confirmationLink);

                    if (!emailSend)
                    {
                        response.Success = false;
                        response.Message = "Failed to send email.";
                        return response;
                    }

                    response.Success = true;
                    response.Message = "New confirmation Email has been sent.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Confirmation token is still valid. Please double check your Email.";
                }
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = "Failed to send Email.";
                response.ErrorMessages = new List<string> { e.Message };
            }
            return response;
        }

        public async Task<ServiceResponse<RegisterDTO>> CreateStaffAccountAsync(int userId, RegisterDTO register)
        {
            var response = new ServiceResponse<RegisterDTO>();
            try
            {
                var user = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
                if (user == null || user.Role != UserEnum.ADMIN)
                {
                    response.Success = false;
                    response.Message = "You are not allowed.";
                    return response;
                }
                var existEmail = await _unitOfWork.UserRepo.CheckEmailAddressExisted(register.Email);
                if (existEmail)
                {
                    response.Success = false;
                    response.Message = "Email is already existed";
                    return response;
                }
                var staffAccount = _mapper.Map<User>(register);
                staffAccount.Role = UserEnum.STAFF;
                staffAccount.Password = HashPassWithSHA256.HashWithSHA256(register.Password);
                staffAccount.CreatedDatetime = DateTime.UtcNow;

                await _unitOfWork.UserRepo.AddAsync(staffAccount);

                var token = new Token
                {
                    TokenValue = "success",
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow,
                    Type = "confirmation",
                    UserId = staffAccount.UserId
                };

                await _unitOfWork.TokenRepo.AddAsync(token);

                response.Success = true;
                response.Message = "Staff account created successfully.";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create account: {ex.Message}";
                return response;
            }
        }

        public async Task<IActionResult?> CheckIfUserHasPermissionsToUpdateOrDeleteByProjectId(int projectId, User? user = null)
        {
            try
            {
                if (user == null || !(user.UserId > 0))
                {
                    return new UnauthorizedResult();
                }
                if (user.IsDeleted || !user.IsVerified)
                {
                    //return new ForbidResult();
                    var result = new { StatusCode = StatusCodes.Status403Forbidden, Message = "This account is either deleted or unverified." };
                    return new ObjectResult(result);
                }
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
                if (existingProject == null)
                {
                    //return new NotFoundResult();
                    var result = new { StatusCode = StatusCodes.Status404NotFound, Message = "The project associated with the request cannot be found." };
                    return new NotFoundObjectResult(result);
                }
                if (user.Role == UserEnum.CUSTOMER)
                {
                    if (user.UserId != existingProject.CreatorId)
                    {
                        var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(user.UserId, existingProject.ProjectId);
                        if (existingCollaborator == null || (existingCollaborator.Role != Domain.Enums.CollaboratorEnum.ADMINISTRATOR && existingCollaborator.Role == Domain.Enums.CollaboratorEnum.EDITOR))
                        {
                            //return new ForbidResult();
                            var result = new { StatusCode = StatusCodes.Status403Forbidden, Message = "This request is forbidden to the customer." };
                            return new ObjectResult(result);
                        }
                    }
                }
                else
                {
                    if (user.Role == UserEnum.STAFF && user.UserId != existingProject.MonitorId)
                    {
                        //return new ForbidResult();
                        var result = new { StatusCode = StatusCodes.Status403Forbidden, Message = "This request is forbidden to the staff." };
                        return new ObjectResult(result);
                    }
                }

                return null;
            }
            catch
            {
            }
            return new BadRequestResult();
        }

        public async Task<IActionResult?> CheckIfUserCanGetByProjectId(int projectId, User? user = null)
        {
            try
            {
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
                if (existingProject == null || ((user == null || user.Role == UserEnum.CUSTOMER) && existingProject.Status == Domain.Enums.ProjectStatusEnum.DELETED))
                {
                    var result = new { StatusCode = StatusCodes.Status404NotFound, Message = "The project associated with the request cannot be found." };
                    return new ObjectResult(result);
                }
                if (user == null && existingProject.Status == Domain.Enums.ProjectStatusEnum.INVISIBLE)
                {
                    return new UnauthorizedResult();
                }
                if (user != null && user.Role == UserEnum.CUSTOMER && existingProject.Status == Domain.Enums.ProjectStatusEnum.INVISIBLE)
                {
                    if (user.IsDeleted && !user.IsVerified)
                    {
                        //return new ForbidResult();
                        var result = new { StatusCode = StatusCodes.Status403Forbidden, Message = "This account is either deleted or unverified." };
                        return new ObjectResult(result);
                    }
                    else if (user.UserId != existingProject.CreatorId)
                    {
                        var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(user.UserId, existingProject.ProjectId);
                        if (existingCollaborator == null)
                        {
                            //return new ForbidResult();
                            var result = new { StatusCode = StatusCodes.Status403Forbidden, Message = "This request is forbidden to the customer." };
                            return new ObjectResult(result);
                        }
                    }
                }
                return null;
            }
            catch
            {
            }
            return new BadRequestResult();
        }

        public async Task<ServiceResponse<string>> ForgetPasswordAsync(string email)
        {
            var response = new ServiceResponse<string>();
            try
            {
                // Check if the user exists
                var user = await _unitOfWork.UserRepo.GetByEmailAsync(email);
                if (user == null || user.IsDeleted)
                {
                    response.Success = false;
                    response.Message = "User with this email does not exist.";
                    return response;
                }

                // Generate a password reset token
                var resetToken = new Token
                {
                    TokenValue = Guid.NewGuid().ToString(),
                    Type = "password_reset",
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    UserId = user.UserId
                };

                // Save the token to the database
                await _unitOfWork.TokenRepo.AddAsync(resetToken);

                // Construct the password reset link
                var resetLink = $"https://marvelous-gentleness-production.up.railway.app/swagger/confirm?token={resetToken.TokenValue}";

                // Send the reset link via email
                var emailSent = await EmailSender.SendPasswordResetEmail(email, resetLink);
                if (!emailSent)
                {
                    response.Success = false;
                    response.Message = "Failed to send password reset email.";
                    return response;
                }

                response.Success = true;
                response.Message = "Password reset email sent successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while processing the request.";
                response.ErrorMessages = new List<string> { ex.Message };
            }

            return response;
        }
        public async Task<ServiceResponse<string>> ResetPasswordAsync(string token, string newPassword)
        {
            var response = new ServiceResponse<string>();
            try
            {
                // Validate the token
                var resetToken = await _unitOfWork.TokenRepo.GetTokenByValueAsync(token);
                if (resetToken == null || resetToken.ExpiresAt < DateTime.UtcNow)
                {
                    response.Success = false;
                    response.Message = "Invalid or expired token.";
                    return response;
                }

                // Retrieve the user associated with the token
                var user = await _unitOfWork.UserRepo.GetByIdAsync(resetToken.UserId);
                if (user == null || user.IsDeleted)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                // Update the user's password
                user.Password = HashPassWithSHA256.HashWithSHA256(newPassword);
                await _unitOfWork.UserRepo.UpdateAsync(user);

                // Invalidate the token
                await _unitOfWork.TokenRepo.RemoveAsync(resetToken);

                response.Success = true;
                response.Message = "Password reset successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while resetting the password.";
                response.ErrorMessages = new List<string> { ex.Message };
            }

            return response;
        }

    }
}
