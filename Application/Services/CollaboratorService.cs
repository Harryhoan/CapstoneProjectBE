using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels.CollaboratorDTO;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Application.Services
{
    public class CollaboratorService : ICollaboratorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CollaboratorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<CollaboratorDTO>> CreateCollaborator(CreateCollaboratorDTO createCollaboratorDTO, Domain.Entities.User user)
        {
            var response = new ServiceResponse<CollaboratorDTO>();

            try
            {
                var validationContext = new ValidationContext(createCollaboratorDTO);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(createCollaboratorDTO, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                var existingUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", createCollaboratorDTO.UserId);
                if (existingUser == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }
                if (existingUser.IsDeleted)
                {
                    response.Success = false;
                    response.Message = "User deleted";
                    return response;
                }
                if (!existingUser.IsVerified)
                {
                    response.Success = false;
                    response.Message = "User unverified";
                    return response;
                }
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", createCollaboratorDTO.ProjectId);
                if (existingProject == null)
                {
                    response.Success = false;
                    response.Message = "Project not found";
                    return response;
                }
                if (existingProject.CreatorId != user.UserId)
                {
                    var userCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectIdAsNoTracking(user.UserId, existingProject.ProjectId);
                    if (userCollaborator == null || userCollaborator.Role != Domain.Enums.CollaboratorEnum.ADMINISTRATOR)
                    {
                        response.Success = false;
                        response.Message = "This request is not permitted";
                        return response;
                    }
                }
                var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(existingUser.UserId, existingProject.ProjectId);
                if (existingCollaborator != null)
                {
                    response.Success = false;
                    response.Message = "Collaborator already exists";
                    return response;
                }
                Collaborator collaborator = new()
                {
                    ProjectId = existingProject.ProjectId,
                    UserId = existingUser.UserId,
                    Role = createCollaboratorDTO.Role
                };
                await _unitOfWork.CollaboratorRepo.AddAsync(collaborator);
                collaborator.Project = existingProject;
                collaborator.User = existingUser;
                response.Data = _mapper.Map<CollaboratorDTO>(collaborator);
                response.Success = true;
                response.Message = "Collaborator created successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create collaborator: {ex.Message}";
            }
            return response;
        }
        public async Task<ServiceResponse<CollaboratorDTO>> CreateCollaboratorByUserEmail(CreateCollaboratorByEmailDTO createCollaboratorDTO, Domain.Entities.User user)
        {
            var response = new ServiceResponse<CollaboratorDTO>();

            try
            {
                var validationContext = new ValidationContext(createCollaboratorDTO);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(createCollaboratorDTO, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                var existingUser = await _unitOfWork.UserRepo.GetByEmailAsync(createCollaboratorDTO.Email);
                if (existingUser == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }
                if (existingUser.IsDeleted)
                {
                    response.Success = false;
                    response.Message = "User deleted";
                    return response;
                }
                if (!existingUser.IsVerified)
                {
                    response.Success = false;
                    response.Message = "User unverified";
                    return response;
                }
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", createCollaboratorDTO.ProjectId);
                if (existingProject == null)
                {
                    response.Success = false;
                    response.Message = "Project not found";
                    return response;
                }
                if (existingProject.CreatorId != user.UserId)
                {
                    var userCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectIdAsNoTracking(user.UserId, existingProject.ProjectId);
                    if (userCollaborator == null || userCollaborator.Role != Domain.Enums.CollaboratorEnum.ADMINISTRATOR)
                    {
                        response.Success = false;
                        response.Message = "This request is not permitted";
                        return response;
                    }
                }
                var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(existingUser.UserId, existingProject.ProjectId);
                if (existingCollaborator != null)
                {
                    response.Success = false;
                    response.Message = "Collaborator already exists";
                    return response;
                }
                Collaborator collaborator = new()
                {
                    ProjectId = existingProject.ProjectId,
                    UserId = existingUser.UserId,
                    Role = createCollaboratorDTO.Role
                };
                await _unitOfWork.CollaboratorRepo.AddAsync(collaborator);
                collaborator.Project = existingProject;
                collaborator.User = existingUser;
                response.Data = _mapper.Map<CollaboratorDTO>(collaborator);
                response.Success = true;
                response.Message = "Collaborator created successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create collaborator: {ex.Message}";
            }
            return response;
        }

        private async Task<List<Collaborator>?> FilterCollaboratorsByProjectId(int projectId)
        {
            var collaborators = await _unitOfWork.CollaboratorRepo.GetCollaboratorsByProjectId(projectId);
            var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
            if (existingProject == null)
            {
                await _unitOfWork.CollaboratorRepo.RemoveAll(collaborators);
                return null;
            }
            int i = 0;
            while (i < collaborators.Count)
            {
                var existingUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", collaborators[i].UserId);
                if (existingUser == null /*|| existingUser.IsDeleted*/)
                {
                    await _unitOfWork.CollaboratorRepo.RemoveAsync(collaborators[i]);
                    collaborators.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            return collaborators;
        }

        private async Task<List<Collaborator>?> FilterCollaboratorsByUserId(int userId, User? user = null)
        {
            var collaborators = await _unitOfWork.CollaboratorRepo.GetCollaboratorsByUserId(userId);
            var existingUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
            if (existingUser == null /*|| existingUser.IsDeleted*/)
            {
                await _unitOfWork.CollaboratorRepo.RemoveAll(collaborators);
                return null;
            }
            int i = 0;
            while (i < collaborators.Count)
            {
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", collaborators[i].ProjectId);
                if (existingProject == null)
                {
                    await _unitOfWork.CollaboratorRepo.RemoveAsync(collaborators[i]);
                    collaborators.RemoveAt(i);
                }
                else if (existingProject.Status == Domain.Enums.ProjectEnum.INVISIBLE)
                {
                    if (user != null)
                    {
                        if (user.Role == UserEnum.CUSTOMER && user.UserId != userId && user.UserId != existingProject.CreatorId)
                        {
                            var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectIdAsNoTracking(user.UserId, existingProject.ProjectId);
                            if (existingCollaborator == null)
                            {
                                collaborators.RemoveAt(i);
                            }
                        }
                    }
                }
                else
                {
                    i++;
                }
            }
            return collaborators;
        }

        public async Task<ServiceResponse<PaginationModel<CollaboratorDTO>>> GetPaginatedCollaborators(int page = 1, int pageSize = 20)
        {
            var response = new ServiceResponse<PaginationModel<CollaboratorDTO>>();

            try
            {
                var collaborators = await _unitOfWork.CollaboratorRepo.GetCollaboratorsIncludeUserAndProject();
                var collaboratorDTOs = _mapper.Map<List<CollaboratorDTO>>(collaborators);
                response.Data = await Pagination.GetPagination(collaboratorDTOs, page, pageSize);
                response.Success = true;
                if (!response.Data.ListData.Any())
                {
                    response.Message = "No collaborator found";
                }
                else
                {
                    response.Message = "Retrieve collaborator(s) successfully";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get collaborators: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<List<CollaboratorDTO>>> GetCollaborators()
        {
            var response = new ServiceResponse<List<CollaboratorDTO>>();

            try
            {
                var collaborators = await _unitOfWork.CollaboratorRepo.GetCollaboratorsIncludeUserAndProject();
                var collaboratorDTOs = _mapper.Map<List<CollaboratorDTO>>(collaborators);
                response.Data = collaboratorDTOs;
                response.Success = true;
                if (!response.Data.Any())
                {
                    response.Message = "No collaborator found";
                }
                else
                {
                    response.Message = "Retrieve collaborator(s) successfully";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get collaborators: {ex.Message}";
            }
            return response;
        }


        public async Task<ServiceResponse<PaginationModel<UserCollaboratorDTO>>> GetPaginatedCollaboratorsByProjectId(int projectId, int page = 1, int pageSize = 20)
        {
            var response = new ServiceResponse<PaginationModel<UserCollaboratorDTO>>();

            try
            {
                var collaborators = await FilterCollaboratorsByProjectId(projectId);
                if (collaborators == null)
                {
                    response.Success = false;
                    response.Message = "Project not found";
                    return response;
                }
                var collaboratorDTOs = _mapper.Map<List<UserCollaboratorDTO>>(collaborators);
                response.Data = await Pagination.GetPagination(collaboratorDTOs, page, pageSize);
                response.Success = true;
                if (!response.Data.ListData.Any())
                {
                    response.Message = "No collaborator found";
                }
                else
                {
                    response.Message = "Retrieve collaborator(s) successfully";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get collaborators: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<List<UserCollaboratorDTO>>> GetCollaboratorsByProjectId(int projectId)
        {
            var response = new ServiceResponse<List<UserCollaboratorDTO>>();

            try
            {
                var collaborators = await FilterCollaboratorsByProjectId(projectId);
                if (collaborators == null)
                {
                    response.Success = false;
                    response.Message = "Project not found";
                    return response;
                }
                var collaboratorDTOs = _mapper.Map<List<UserCollaboratorDTO>>(collaborators);
                response.Data = collaboratorDTOs;
                response.Success = true;
                if (!response.Data.Any())
                {
                    response.Message = "No collaborator found";
                }
                else
                {
                    response.Message = "Retrieve collaborator(s) successfully";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get collaborators: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<PaginationModel<ProjectCollaboratorDTO>>> GetPaginatedCollaboratorsByUserId(int userId, int page = 1, int pageSize = 20, User? user = null)
        {
            var response = new ServiceResponse<PaginationModel<ProjectCollaboratorDTO>>();

            try
            {
                var collaborators = await FilterCollaboratorsByUserId(userId, user);
                if (collaborators == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }

                var collaboratorDTOs = _mapper.Map<List<ProjectCollaboratorDTO>>(collaborators);
                response.Data = await Pagination.GetPagination(collaboratorDTOs, page, pageSize);
                response.Success = true;
                if (!response.Data.ListData.Any())
                {
                    response.Message = "No collaborator found";
                }
                else
                {
                    response.Message = "Retrieve collaborator(s) successfully";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get collaborators: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<List<ProjectCollaboratorDTO>>> GetCollaboratorsByUserId(int userId, User? user = null)
        {
            var response = new ServiceResponse<List<ProjectCollaboratorDTO>>();

            try
            {
                var collaborators = await FilterCollaboratorsByUserId(userId, user);
                if (collaborators == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }

                var collaboratorDTOs = _mapper.Map<List<ProjectCollaboratorDTO>>(collaborators);
                response.Data = collaboratorDTOs;
                response.Success = true;
                if (!response.Data.Any())
                {
                    response.Message = "No collaborator found";
                }
                else
                {
                    response.Message = "Retrieve collaborator(s) successfully";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get collaborators: {ex.Message}";
            }
            return response;
        }


        public async Task<IActionResult?> CheckIfUserHasPermissionsByProjectId(int projectId, User? user = null)
        {
            try
            {
                //if (user == null || !(user.UserId > 0))
                //{
                //    return new UnauthorizedResult();
                //}
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
                if (existingProject == null || ((user == null || user.Role == UserEnum.CUSTOMER) && existingProject.Status == Domain.Enums.ProjectEnum.DELETED))
                {
                    return new NotFoundResult();
                }
                if (user == null && existingProject.Status == Domain.Enums.ProjectEnum.INVISIBLE)
                {
                    return new UnauthorizedResult();
                }
                if (user != null && user.Role == UserEnum.CUSTOMER && existingProject.Status == Domain.Enums.ProjectEnum.INVISIBLE)
                {
                    if (user.IsDeleted && !user.IsVerified)
                    {
                        return new ForbidResult();
                    }
                    else if (user.UserId != existingProject.CreatorId)
                    {
                        var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(user.UserId, existingProject.ProjectId);
                        if (existingCollaborator == null)
                        {
                            return new ForbidResult();
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

        public async Task<IActionResult?> CheckIfUserCanRemoveByProjectId(int userId, int projectId, User? user = null)
        {
            try
            {
                if (user == null || !(user.UserId > 0))
                {
                    return new UnauthorizedResult();
                }
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
                if (existingProject == null || (user.Role == UserEnum.CUSTOMER && existingProject.Status == Domain.Enums.ProjectEnum.DELETED))
                {
                    return new NotFoundResult();
                }
                var existingUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
                if (existingUser == null || existingUser.IsDeleted)
                {
                    return new NotFoundResult();
                }
                if (user.Role == UserEnum.CUSTOMER && existingUser.UserId != user.UserId)
                {
                    var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(user.UserId, existingProject.ProjectId);
                    if ((existingCollaborator == null && user.UserId != existingProject.CreatorId) || (existingCollaborator != null && existingCollaborator.Role != Domain.Enums.CollaboratorEnum.ADMINISTRATOR))
                    {
                        return new ForbidResult();
                    }
                }
                else
                {
                    if (user.Role == UserEnum.STAFF)
                    {
                        if (user.UserId != existingProject.MonitorId)
                        {
                            return new ForbidResult();
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

        public async Task<IActionResult?> CheckIfUserCanUpdateByProjectId(CollaboratorEnum role, int userId, int projectId, User? user = null)
        {
            try
            {
                if (user == null || !(user.UserId > 0))
                {
                    return new UnauthorizedResult();
                }
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
                if (existingProject == null || (user.Role == UserEnum.CUSTOMER && existingProject.Status == Domain.Enums.ProjectEnum.DELETED))
                {
                    return new NotFoundResult();
                }
                var existingUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
                if (existingUser == null || existingUser.IsDeleted)
                {
                    return new NotFoundResult();
                }
                if (user.Role == UserEnum.CUSTOMER)
                {
                    var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(user.UserId, existingProject.ProjectId);
                    //if ((existingCollaborator == null && user.UserId != existingProject.CreatorId) || (existingCollaborator != null && ((existingCollaborator.UserId != user.UserId && existingCollaborator.Role != CollaboratorEnum.ADMINISTRATOR) || (existingCollaborator.UserId == userId && (int) role < (int)existingCollaborator.Role))))
                    //{
                    //    return new ForbidResult();
                    //}
                    if (existingCollaborator == null)
                    {
                        if (user.UserId != existingProject.CreatorId)
                        {
                            return new ForbidResult();
                        }
                    }
                    else if (user.UserId != existingProject.CreatorId)
                    {
                        if (existingCollaborator.UserId != user.UserId)
                        {
                            if (existingCollaborator.Role != CollaboratorEnum.ADMINISTRATOR)
                            {
                                return new ForbidResult();

                            }
                        }
                        else if ((int)role < (int)existingCollaborator.Role)
                        {
                            return new ForbidResult();
                        }
                    }
                }
                else
                {
                    if (user.Role == UserEnum.STAFF)
                    {
                        if (user.UserId != existingProject.MonitorId)
                        {
                            return new ForbidResult();
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


        public async Task<ServiceResponse<CollaboratorDTO>> UpdateCollaborator(CreateCollaboratorDTO createCollaboratorDTO)
        {
            var response = new ServiceResponse<CollaboratorDTO>();

            try
            {
                var validationContext = new ValidationContext(createCollaboratorDTO);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(createCollaboratorDTO, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                var existingUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", createCollaboratorDTO.UserId);
                if (existingUser == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", createCollaboratorDTO.ProjectId);
                if (existingProject == null)
                {
                    response.Success = false;
                    response.Message = "Project not found";
                    return response;
                }
                var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(existingUser.UserId, existingProject.ProjectId);
                if (existingCollaborator == null)
                {
                    response.Success = false;
                    response.Message = "Collaborator not found";
                    return response;
                }
                existingCollaborator.Role = createCollaboratorDTO.Role;
                await _unitOfWork.CollaboratorRepo.UpdateAsync(existingCollaborator);
                existingCollaborator.Project = existingProject;
                existingCollaborator.User = existingUser;
                response.Data = _mapper.Map<CollaboratorDTO>(existingCollaborator);
                response.Success = true;
                response.Message = "Collaborator updated successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create collaborator: {ex.Message}";
            }
            return response;
        }


        public async Task<ServiceResponse<string>> RemoveCollaborator(int userId, int projectId)
        {
            var response = new ServiceResponse<string>();

            try
            {
                var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(userId, projectId);
                if (existingCollaborator == null)
                {
                    response.Success = false;
                    response.Message = "Collaborator not found";
                    return response;
                }
                await _unitOfWork.CollaboratorRepo.RemoveAsync(existingCollaborator);
                //existingPost.Status = "Deleted";
                //await _unitOfWork.PostRepo.Update(existingPost);
                response.Data = "Collaborator removed successfully";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to remove Collaborator: {ex.Message}";
            }

            return response;
        }

    }
}
