using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels.CollaboratorDTO;
using Application.ViewModels.CommentDTO;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        if (user.Role == "Customer" && user.UserId != userId && user.UserId != existingProject.CreatorId)
                        {
                            var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectIdAsNoTracking(user.UserId, existingProject.ProjectId);
                            if(existingCollaborator == null )
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
                //    return new UnauthorizedObjectResult("This request is not authorized.");
                //}
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
                if (existingProject == null || ((user == null || user.Role == "Customer") && existingProject.Status == Domain.Enums.ProjectEnum.DELETED))
                {
                    return new NotFoundObjectResult("The project cannot be found.");
                }
                if (user == null && existingProject.Status == Domain.Enums.ProjectEnum.INVISIBLE)
                {
                    return new UnauthorizedObjectResult("This request is not authorized.");
                }
                if (user != null && user.Role == "Customer" && existingProject.Status == Domain.Enums.ProjectEnum.INVISIBLE)
                {
                    var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(user.UserId, existingProject.ProjectId);
                    if (existingCollaborator == null && user.UserId != existingProject.CreatorId)
                    {
                        return new ForbidResult("This request is forbidden.");
                    }
                }
                return null;
            }
            catch
            {
            }
            return new BadRequestObjectResult("This request cannot be processed.");
        }

        public async Task<IActionResult?> CheckIfUserCanRemoveByProjectId(int userId, int projectId, User? user = null)
        {
            try
            {
                if (user == null || !(user.UserId > 0))
                {
                    return new UnauthorizedObjectResult("This request is not authorized.");
                }
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
                if (existingProject == null || (user.Role == "Customer" && existingProject.Status == Domain.Enums.ProjectEnum.DELETED))
                {
                    return new NotFoundObjectResult("The project cannot be found.");
                }
                var existingUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
                if (existingUser == null || existingUser.IsDeleted)
                {
                    return new NotFoundObjectResult("The user cannot be found.");
                }
                if (user.Role == "Customer" && existingUser.UserId != user.UserId)
                {
                    var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(user.UserId, existingProject.ProjectId);
                    if ((existingCollaborator == null && user.UserId != existingProject.CreatorId) || (existingCollaborator != null && existingCollaborator.Role != Domain.Enums.CollaboratorEnum.ADMINISTRATOR))
                    {
                        return new ForbidResult("This request is forbidden.");
                    }
                }
                else
                {
                    if (user.Role == "Staff")
                    {
                        if (user.UserId != existingProject.MonitorId)
                        {
                            return new ForbidResult("This request is forbidden.");
                        }
                    }
                }
                return null;
            }
            catch
            {
            }
            return new BadRequestObjectResult("This request cannot be processed.");
        }

        public async Task<IActionResult?> CheckIfUserCanUpdateByProjectId(CollaboratorEnum role, int userId, int projectId, User? user = null)
        {
            try
            {
                if (user == null || !(user.UserId > 0))
                {
                    return new UnauthorizedObjectResult("This request is not authorized.");
                }
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
                if (existingProject == null || (user.Role == "Customer" && existingProject.Status == Domain.Enums.ProjectEnum.DELETED))
                {
                    return new NotFoundObjectResult("The project cannot be found.");
                }
                var existingUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
                if (existingUser == null || existingUser.IsDeleted)
                {
                    return new NotFoundObjectResult("The user cannot be found.");
                }
                if (user.Role == "Customer")
                {
                    var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(user.UserId, existingProject.ProjectId);
                    //if ((existingCollaborator == null && user.UserId != existingProject.CreatorId) || (existingCollaborator != null && ((existingCollaborator.UserId != user.UserId && existingCollaborator.Role != CollaboratorEnum.ADMINISTRATOR) || (existingCollaborator.UserId == userId && (int) role < (int)existingCollaborator.Role))))
                    //{
                    //    return new ForbidResult("This request is forbidden.");
                    //}
                    if (existingCollaborator == null)
                    {
                        if (user.UserId != existingProject.CreatorId)
                        {
                            return new ForbidResult("This request is forbidden.");
                        }
                    }
                    else if (user.UserId != existingProject.CreatorId)
                    {
                        if (existingCollaborator.UserId != user.UserId)
                        {
                            if (existingCollaborator.Role != CollaboratorEnum.ADMINISTRATOR)
                            {
                                return new ForbidResult("This request is forbidden.");

                            }
                        }
                        else if ((int)role < (int)existingCollaborator.Role)
                        {
                            return new ForbidResult("This request is forbidden.");
                        }
                    }
                }
                else
                {
                    if (user.Role == "Staff")
                    {
                        if (user.UserId != existingProject.MonitorId)
                        {
                            return new ForbidResult("This request is forbidden.");
                        }
                    }
                }
                return null;
            }
            catch
            {
            }
            return new BadRequestObjectResult("This request cannot be processed.");
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
                Collaborator collaborator = new()
                {
                    ProjectId = existingProject.ProjectId,
                    UserId = existingUser.UserId,
                    Role = createCollaboratorDTO.Role
                };
                await _unitOfWork.CollaboratorRepo.UpdateAsync(collaborator);
                collaborator.Project = existingProject;
                collaborator.User = existingUser;
                response.Data = _mapper.Map<CollaboratorDTO>(collaborator);
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
