﻿using Application.ServiceResponse;
using Application.ViewModels.UserDTO;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Application.IService
{
    public interface IAuthenService
    {
        public Task<ServiceResponse<RegisterDTO>> RegisterAsync(RegisterDTO userObject);
        public Task<TokenResponse<string>> LoginAsync(LoginUserDTO userObject);
        public Task<User?> GetUserByTokenAsync(ClaimsPrincipal claims);
        public Task<ServiceResponse<string>> ResendConfirmationTokenAsync(string email);
        public Task<ServiceResponse<RegisterDTO>> CreateStaffAccountAsync(int userId, RegisterDTO register);
        public Task<IActionResult?> CheckIfUserCanGetByProjectId(int projectId, User? user = null);
        public Task<IActionResult?> CheckIfUserHasCreatorPermissionsToUpdateOrDeleteByProjectId(int projectId, User? user = null);
        public Task<IActionResult?> CheckIfUserHasPermissionsToUpdateOrDeleteByProjectId(int projectId, User? user = null);
        public Task<ServiceResponse<string>> ForgetPasswordAsync(string email);
        public Task<ServiceResponse<string>> ResetPasswordAsync(string token, string newPassword);
    }
}
