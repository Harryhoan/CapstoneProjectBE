using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels.PlatformDTO;
using Application.ViewModels.ProjectDTO;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Application.Services
{
    public class PlatformService : IPlatformService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PlatformService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<List<PlatformDTO>>> GetAllPlatformAsync()
        {
            var response = new ServiceResponse<List<PlatformDTO>>();
            try
            {
                var platforms = await _unitOfWork.PlatformRepo.GetAllAsync();
                if (platforms == null)
                {
                    response.Success = false;
                    response.Message = "Platform not found";
                    return response;
                }
                var platformDTOs = _mapper.Map<List<PlatformDTO>>(platforms);
                response.Data = platformDTOs;
                response.Success = true;
            }catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get platforms: {ex.Message}";
            }
            return response;
        }
        public async Task<ServiceResponse<List<ProjectDto>>> GetAllProjectByPlatformId(int platformId)
        {
            var response = new ServiceResponse<List<ProjectDto>>();
            try
            {
                // Retrieve all ProjectPlatform entities for the given platformId
                var projectPlatforms = await _unitOfWork.ProjectPlatformRepo.GetAllProjectByPlatformId(platformId);

                if (projectPlatforms == null || !projectPlatforms.Any())
                {
                    response.Success = false;
                    response.Message = "No projects found for the given platform.";
                    return response;
                }

                // Map the associated projects to ProjectDto
                var projectList = projectPlatforms
                    .Where(pp => pp.Project != null) // Ensure the Project is not null
                    .Select(pp => _mapper.Map<ProjectDto>(pp.Project))
                    .ToList();

                response.Data = projectList;
                response.Success = true;
                response.Message = "Projects retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return response;
        }
        public async Task<ServiceResponse<List<PlatformDTO>>> GetAllPlatformByProjectId(int projectId)
        {
            var response = new ServiceResponse<List<PlatformDTO>>();
            try
            {
                var platforms = await _unitOfWork.ProjectPlatformRepo.GetAllPlatformByProjectId(projectId);
                if (platforms == null)
                {
                    response.Success = false;
                    response.Message = "Platform not found";
                    return response;
                }
                var platformDTOs = _mapper.Map<List<PlatformDTO>>(platforms);
                response.Data = platformDTOs;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get platforms: {ex.Message}";
            }
            return response;
        }
        public async Task<ServiceResponse<PlatformDTO>> GetPlatformByPlatformId(int platformId)
        {
            var response = new ServiceResponse<PlatformDTO>();
            try
            {
                var platforms = await _unitOfWork.PlatformRepo.GetByIdAsync(platformId);
                if (platforms == null)
                {
                    response.Success = false;
                    response.Message = "Platform not found";
                    return response;
                }
                var platformDTOs = _mapper.Map<PlatformDTO>(platforms);
                response.Data = platformDTOs;
                response.Success = true;
                response.Message = "Platform retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get platforms: {ex.Message}";
            }
            return response;
        }
        public async Task<ServiceResponse<int>> CreatePlatform(CreatePlatformDTO createPlatformDTO)
        {
            var response = new ServiceResponse<int>();

            try
            {
                var validationContext = new ValidationContext(createPlatformDTO);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(createPlatformDTO, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                Platform platform = new Platform();
                platform.Name = createPlatformDTO.Name;
                platform.Description = createPlatformDTO.Description;
                await _unitOfWork.PlatformRepo.AddAsync(platform);
                response.Data = platform.PlatformId;
                response.Success = true;
                response.Message = "Platform created successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create platform: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<PlatformDTO>> GetPlatformById(int platformId)
        {
            var response = new ServiceResponse<PlatformDTO>();
            try
            {
                var platform = await _unitOfWork.PlatformRepo.GetByIdAsync(platformId);
                if (platform == null)
                {
                    response.Success = false;
                    response.Message = "Platform not found";
                    return response;
                }

                response.Data = _mapper.Map<PlatformDTO>(platform);
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create platform: {ex.Message}";
            }
            return response;

        }

        public async Task<ServiceResponse<List<PlatformDTO>>> GetPlatforms(string? query = null)
        {
            var response = new ServiceResponse<List<PlatformDTO>>();

            try
            {
                var platforms = string.IsNullOrEmpty(query) ? await _unitOfWork.PlatformRepo.GetAllAsNoTrackingAsync() : await _unitOfWork.PlatformRepo.GetPlatformsByNameOrDescriptionAsNoTracking(query);
                if (platforms == null)
                {
                    response.Success = false;
                    response.Message = "Platform not found";
                    return response;
                }

                var platformDTOs = _mapper.Map<List<PlatformDTO>>(platforms);
                response.Data = platformDTOs;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get platforms: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<PaginationModel<PlatformDTO>>> GetPaginatedPlatforms(string? query = null, int page = 1, int pageSize = 20)
        {
            var response = new ServiceResponse<PaginationModel<PlatformDTO>>();

            try
            {
                var platforms = string.IsNullOrEmpty(query) ? await _unitOfWork.PlatformRepo.GetAllAsNoTrackingAsync() : await _unitOfWork.PlatformRepo.GetPlatformsByNameOrDescriptionAsNoTracking(query);
                if (platforms == null)
                {
                    response.Success = false;
                    response.Message = "Platform not found";
                    return response;
                }

                var platformDTOs = _mapper.Map<List<PlatformDTO>>(platforms);
                response.Data = await Pagination.GetPagination(platformDTOs, page, pageSize);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get platforms: {ex.Message}";
            }
            return response;
        }


        public async Task<ServiceResponse<string>> UpdatePlatform(int platformId, CreatePlatformDTO createPlatformDTO)
        {
            var response = new ServiceResponse<string>();

            try
            {
                var validationContext = new ValidationContext(createPlatformDTO);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(createPlatformDTO, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                var existingPlatform = await _unitOfWork.PlatformRepo.GetByIdAsync(platformId);
                if (existingPlatform == null)
                {
                    response.Success = false;
                    response.Message = "Platform not found";
                    return response;
                }
                existingPlatform.Name = createPlatformDTO.Name;
                existingPlatform.Description = createPlatformDTO.Description;

                await _unitOfWork.PlatformRepo.UpdateAsync(existingPlatform);
                response.Data = "Platform updated successfully";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to update platform: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<string>> RemovePlatform(int platformId)
        {
            var response = new ServiceResponse<string>();

            try
            {
                var existingPlatform = await _unitOfWork.PlatformRepo.GetByIdAsync(platformId);
                if (existingPlatform == null)
                {
                    response.Success = false;
                    response.Message = "Platform not found";
                    return response;
                }

                var existingProjectPlatforms = await _unitOfWork.ProjectPlatformRepo.GetProjectPlatformsByPlatformId(platformId);
                if (existingProjectPlatforms != null && existingProjectPlatforms.Count > 0)
                {
                    await _unitOfWork.ProjectPlatformRepo.RemoveAll(existingProjectPlatforms);
                }

                await _unitOfWork.PlatformRepo.RemoveAsync(existingPlatform);
                response.Data = "Platform removed successfully";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to remove platform: {ex.Message}";
            }

            return response;
        }

        //public async Task<IActionResult?> CheckIfUserHasPermissionsByProjectId(int projectId, User? user = null)
        //{
        //    if (user == null || !(user.UserId > 0))
        //    {
        //        return new UnauthorizedResult();
        //    }
        //    var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
        //    if (existingProject == null)
        //    {
        //        return new NotFoundResult();
        //    }
        //    if (user.Role == UserEnum.CUSTOMER)
        //    {
        //        if (user.UserId != existingProject.CreatorId)
        //        {
        //            var existingCollaborator = await _unitOfWork.CollaboratorRepo.GetCollaboratorByUserIdAndProjectId(user.UserId, existingProject.ProjectId);
        //            if (existingCollaborator == null || (existingCollaborator.Role != Domain.Enums.CollaboratorEnum.ADMINISTRATOR && existingCollaborator.Role == Domain.Enums.CollaboratorEnum.EDITOR))
        //            {
        //                return new ForbidResult();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (user.Role == UserEnum.STAFF && user.UserId != existingProject.MonitorId)
        //        {
        //            return new ForbidResult();
        //        }
        //    }

        //    return null;
        //}

        public async Task<ServiceResponse<int>> CreateProjectPlatform(ProjectPlatformDTO projectPlatformDTO)
        {
            var response = new ServiceResponse<int>();

            try
            {
                var validationContext = new ValidationContext(projectPlatformDTO);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(projectPlatformDTO, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                var existingPlatform = await _unitOfWork.PlatformRepo.GetByIdAsync(projectPlatformDTO.PlatformId);
                if (existingPlatform == null)
                {
                    response.Success = false;
                    response.Message = "Platform not found";
                    return response;
                }
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdAsync(projectPlatformDTO.ProjectId);
                if (existingProject == null)
                {
                    response.Success = false;
                    response.Message = "Project not found";
                    return response;
                }

                var existingProjectPlatform = await _unitOfWork.ProjectPlatformRepo.GetProjectPlatformByProjectIdAndPlatformId(projectPlatformDTO.ProjectId, projectPlatformDTO.PlatformId);
                if (existingProjectPlatform != null)
                {
                    response.Success = false;
                    response.Message = "Platform already added to project";
                    return response;
                }

                ProjectPlatform projectPlatform = new()
                {
                    ProjectId = projectPlatformDTO.ProjectId,
                    PlatformId = projectPlatformDTO.PlatformId
                };
                await _unitOfWork.ProjectPlatformRepo.AddAsync(projectPlatform);
                response.Success = true;
                response.Message = "Platform added to Project successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to add platform to project: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<int>> RemoveProjectPlatform(ProjectPlatformDTO projectPlatformDTO)
        {
            var response = new ServiceResponse<int>();

            try
            {
                var validationContext = new ValidationContext(projectPlatformDTO);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(projectPlatformDTO, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                var existingProjectPlatform = await _unitOfWork.ProjectPlatformRepo.GetProjectPlatformByProjectIdAndPlatformId(projectPlatformDTO.ProjectId, projectPlatformDTO.PlatformId);
                if (existingProjectPlatform == null)
                {
                    response.Success = false;
                    response.Message = "Project Platform not found";
                    return response;
                }

                await _unitOfWork.ProjectPlatformRepo.RemoveAsync(existingProjectPlatform);
                response.Success = true;
                response.Message = "Platform removed from Platform successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to remove platform from project: {ex.Message}";
            }
            return response;
        }

    }
}
