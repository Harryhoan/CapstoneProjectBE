using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels;
using Application.ViewModels.CategoryDTO;
using Application.ViewModels.PlatformDTO;
using Application.ViewModels.ProjectDTO;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Esf;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly Cloudinary _cloudinary;
        private static int _lastAssignedStaffIndex = -1; // Static variable to keep track of the last assigned staff member
        private static int _lastStaffCount = 0; // Static variable to keep track of the last staff count

        public ProjectService(IUnitOfWork unitOfWork, IMapper mapper, IHttpClientFactory httpClientFactory, IOptions<Cloud> config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpClient = httpClientFactory.CreateClient();
            var cloudinaryAccount = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret);
            _cloudinary = new Cloudinary(cloudinaryAccount);
        }

        public async Task<ServiceResponse<List<ProjectDto>>> GetAllProjectByAdminAsync(int userId)
        {
            var response = new ServiceResponse<List<ProjectDto>>();
            try
            {
                var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                IEnumerable<Project> projects;
                if (user.Role == UserEnum.STAFF)
                {
                    projects = await _unitOfWork.ProjectRepo.GetAllProjectByMonitorIdAsync(userId);
                }
                else
                {
                    projects = await _unitOfWork.ProjectRepo.GetAllAsync();
                }

                if (projects == null || !projects.Any())
                {
                    response.Success = true;
                    response.Message = "There are no projects here.";
                    return response;
                }

                var responseData = new List<ProjectDto>();
                foreach (var projectItem in projects)
                {
                    var monitor = await _unitOfWork.UserRepo.GetByIdAsync(projectItem.MonitorId);
                    var creator = await _unitOfWork.UserRepo.GetByIdAsync(projectItem.CreatorId);
                    var category = await _unitOfWork.ProjectCategoryRepo.GetListByProjectIdAsync(projectItem.ProjectId);
                    var platform = await _unitOfWork.ProjectPlatformRepo.GetAllPlatformByProjectId(projectItem.ProjectId);

                    var projectDto = new ProjectDto
                    {
                        ProjectId = projectItem.ProjectId,
                        Thumbnail = projectItem.Thumbnail ?? "Null",
                        Monitor = monitor?.Fullname ?? "Unknown",
                        CreatorId = projectItem.CreatorId,
                        Creator = creator?.Fullname ?? "Unknown",
                        Title = projectItem.Title,
                        Description = projectItem.Description,
                        Status = projectItem.Status,
                        MinimumAmount = projectItem.MinimumAmount,
                        TotalAmount = projectItem.TotalAmount,
                        StartDatetime = projectItem.StartDatetime,
                        EndDatetime = projectItem.EndDatetime,
                        Backers = await _unitOfWork.PledgeRepo.GetBackersByProjectIdAsync(projectItem.ProjectId),
                        Categories = category.Select(c => new ViewCategory
                        {
                            CategoryId = c.CategoryId,
                            Name = c.Category.Name,
                            ParentCategoryId = c.Category.ParentCategoryId,
                            Description = c.Category.Description
                        }).ToList(),
                        Platforms = platform.Select(p => new PlatformDTO
                        {
                            PlatformId = p.PlatformId,
                            Name = p.Platform.Name,
                            Description = p.Platform.Description ?? "Null"
                        }).ToList()
                    };

                    responseData.Add(projectDto);
                }
                response.Data = responseData;
                response.Success = true;
                response.Message = "Get all projects successfully.";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get all projects: {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<ProjectDto>> CreateProject(int userId, CreateProjectDto createProjectDto)
        {
            var response = new ServiceResponse<ProjectDto>();
            var user = await _unitOfWork.UserRepo.GetAllAsync();
            try
            {
                var specificUser = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                if (specificUser == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }
                if (specificUser.IsVerified == false)
                {
                    response.Success = false;
                    response.Message = "Your account is not verified. Missing Phone Number or Payment Account.";
                    return response;
                }
                string apiResponse = await CheckDescriptionAsync(createProjectDto.Description);
                if (apiResponse.Trim().Equals("Có", StringComparison.OrdinalIgnoreCase))
                {
                    response.Success = false;
                    response.Message = "Description contains invalid content.";
                    return response;
                }

                if (createProjectDto.StartDatetime >= createProjectDto.EndDatetime)
                {
                    response.Success = false;
                    response.Message = "Start date must be earlier than end date.";
                    return response;
                }

                var project = _mapper.Map<Project>(createProjectDto);


                var staffUsers = user.Where(u => u.Role == UserEnum.STAFF).ToList();
                if (!staffUsers.Any())
                {
                    response.Success = false;
                    response.Message = "No staff available to assign as monitor.";
                    return response;
                }
                if (staffUsers.Count != _lastStaffCount)
                {
                    _lastAssignedStaffIndex = -1;
                    _lastStaffCount = staffUsers.Count;
                }

                _lastAssignedStaffIndex = (_lastAssignedStaffIndex + 1) % staffUsers.Count;
                var assignedStaff = staffUsers[_lastAssignedStaffIndex];


                project.MonitorId = assignedStaff.UserId;
                project.CreatorId = userId;
                project.TotalAmount = 0;
                project.Status = ProjectStatusEnum.INVISIBLE;
                project.UpdateDatetime = createProjectDto.StartDatetime;
                project.TransactionStatus = TransactionStatusEnum.PENDING;
                await _unitOfWork.ProjectRepo.AddAsync(project);
                var responseData = new ProjectDto
                {
                    ProjectId = project.ProjectId,
                    Monitor = assignedStaff.Fullname,
                    CreatorId = project.CreatorId,
                    Creator = user.FirstOrDefault(u => u.UserId == userId)?.Fullname ?? string.Empty,
                    Title = project.Title,
                    Description = project.Description,
                    Status = project.Status,
                    MinimumAmount = project.MinimumAmount,
                    TotalAmount = project.TotalAmount,
                    StartDatetime = project.StartDatetime,
                    Backers = 0,
                    EndDatetime = project.EndDatetime
                };

                var ConfirmProjectCreatedEmail = await EmailSender.SendProjectConfirmationEmail(specificUser.Fullname, specificUser.Email, assignedStaff.Fullname, assignedStaff.Email, project.Title ?? "Unknown", project.StartDatetime, project.EndDatetime, project.Status);
                if (!ConfirmProjectCreatedEmail)
                {
                    response.Success = false;
                    response.Message = "Error when sending email notification.";
                    return response;
                }
                response.Data = responseData;
                response.Success = true;
                response.Message = "Project created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return response;
        }

        public async Task<ServiceResponse<int>> DeleteProject(int id)
        {
            var response = new ServiceResponse<int>();

            try
            {
                int result = await _unitOfWork.ProjectRepo.DeleteProject(id);

                if (result > 0)
                {
                    response.Data = result;
                    response.Success = true;
                    response.Message = "Project deleted successfully.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Project not found or could not be deleted.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return response;
        }
        public async Task<ServiceResponse<string>> UpdateProjectStoryAsync(int userId, int projectId, string story)
        {
            var response = new ServiceResponse<string>();

            try
            {
                var project = await _unitOfWork.ProjectRepo.GetByIdAsync(projectId);
                var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                if (project == null)
                {
                    response.Success = false;
                    response.Message = "Project not found.";
                    return response;
                }
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }
                if (user.UserId != project.CreatorId)
                {
                    response.Success = false;
                    response.Message = "You are not authorized to update this project.";
                    return response;
                }

                project.Story = story;
                project.UpdateDatetime = DateTime.UtcNow;
                await _unitOfWork.ProjectRepo.UpdateProject(projectId, project);

                response.Success = true;
                response.Message = "Project story updated successfully.";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to update project story: {ex.Message}";
                return response;
            }
        }

        private async Task<IQueryable<Project>> FilterProjects(User? user = null, QueryProjectDto? queryProjectDto = null)
        {
            var query = _unitOfWork.ProjectRepo.GetAllAsNoTrackingAsQueryable();
            if (queryProjectDto != null)
            {
                if (queryProjectDto.CreatorId.HasValue)
                {
                    var existingCreator = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", queryProjectDto.CreatorId.Value);
                    if (existingCreator != null && !(user != null && user.Role == UserEnum.CUSTOMER && existingCreator.IsDeleted))
                    {
                        query = query.Where(p => p.CreatorId == existingCreator.UserId).AsQueryable();
                    }
                    else
                    {
                        query = query.Where(p => p.CreatorId != queryProjectDto.CreatorId.Value).AsQueryable();
                    }
                }
                if (!string.IsNullOrEmpty(queryProjectDto.Title))
                {
                    query = query.Where(p => p.Title != null && p.Title.ToLower().Trim().Contains(queryProjectDto.Title.ToLower().Trim())).AsQueryable();
                }
                if (queryProjectDto.Status != null)
                {
                    query = query.Where(p => p.Status == queryProjectDto.Status).AsQueryable();
                }
                if (queryProjectDto.MinMinimumAmount.HasValue)
                {
                    query = query.Where(p => p.MinimumAmount >= queryProjectDto.MinMinimumAmount.Value);
                }
                if (queryProjectDto.MaxMinimumAmount.HasValue)
                {
                    query = query.Where(p => p.MinimumAmount <= queryProjectDto.MaxMinimumAmount.Value);
                }
                if (queryProjectDto.MaxTotalAmount.HasValue)
                {
                    query = query.Where(p => p.TotalAmount <= queryProjectDto.MaxTotalAmount.Value);
                }
                if (queryProjectDto.MinTotalAmount.HasValue)
                {
                    query = query.Where(p => p.TotalAmount >= queryProjectDto.MinTotalAmount.Value);
                }
                if (queryProjectDto.MaxEndDatetime.HasValue)
                {
                    query = query.Where(p => p.EndDatetime <= queryProjectDto.MaxEndDatetime.Value);
                }
                if (queryProjectDto.MinEndDatetime.HasValue)
                {
                    query = query.Where(p => p.EndDatetime >= queryProjectDto.MinEndDatetime.Value);
                }
                if (queryProjectDto.MinStartDatetime.HasValue)
                {
                    query = query.Where(p => p.StartDatetime >= queryProjectDto.MinStartDatetime.Value);
                }
                if (queryProjectDto.MaxStartDatetime.HasValue)
                {
                    query = query.Where(p => p.StartDatetime <= queryProjectDto.MaxStartDatetime.Value);
                }
                if (queryProjectDto.MinUpdateDatetime.HasValue)
                {
                    query = query.Where(p => p.UpdateDatetime >= queryProjectDto.MinUpdateDatetime.Value);
                }
                if (queryProjectDto.MaxUpdateDatetime.HasValue)
                {
                    query = query.Where(p => p.UpdateDatetime <= queryProjectDto.MaxUpdateDatetime.Value);
                }
                if (queryProjectDto.CategoryIds != null && queryProjectDto.CategoryIds.Any())
                {
                    var projectIds = await _unitOfWork.ProjectCategoryRepo.GetAllAsNoTrackingAsQueryable().Where(pc => queryProjectDto.CategoryIds.Contains(pc.CategoryId)).Select(pc => pc.ProjectId).ToListAsync();
                    query = query.Where(p => projectIds.Contains(p.ProjectId));
                }
                if (queryProjectDto.PlatformIds != null && queryProjectDto.PlatformIds.Any())
                {
                    var projectIds = await _unitOfWork.ProjectPlatformRepo.GetAllAsNoTrackingAsQueryable().Where(pl => queryProjectDto.PlatformIds.Contains(pl.PlatformId)).Select(pc => pc.ProjectId).ToListAsync();
                    query = query.Where(p => projectIds.Contains(p.ProjectId));
                }
            }
            if (user == null)
            {
                query = query.Where(p => p.Status != ProjectStatusEnum.DELETED && p.Status != ProjectStatusEnum.INVISIBLE).AsQueryable();
            }
            else if (user.Role == UserEnum.CUSTOMER)
            {
                query = query.Where(p => p.Status != ProjectStatusEnum.DELETED && !(p.Status == ProjectStatusEnum.INVISIBLE && user.UserId != p.CreatorId)).AsQueryable();
            }
            return query;
        }

        public async Task<ServiceResponse<IEnumerable<ProjectDto>>> GetAllProjects(User? user = null, QueryProjectDto? queryProjectDto = null)
        {
            var response = new ServiceResponse<IEnumerable<ProjectDto>>();

            try
            {
                if (queryProjectDto != null)
                {
                    var validationContext = new ValidationContext(queryProjectDto);
                    var validationResults = new List<ValidationResult>();

                    if (!Validator.TryValidateObject(queryProjectDto, validationContext, validationResults, true))
                    {
                        var errorMessages = validationResults.Select(r => r.ErrorMessage);
                        response.Success = false;
                        response.Message = string.Join("; ", errorMessages);
                        return response;
                    }
                    if (queryProjectDto.MaxMinimumAmount != null && queryProjectDto.MinMinimumAmount != null && queryProjectDto.MaxMinimumAmount < queryProjectDto.MinMinimumAmount)
                    {
                        response.Success = false;
                        response.Message = "Invalid range for the queryable Minimum Amount";
                        return response;
                    }
                    if (queryProjectDto.MaxStartDatetime != null && queryProjectDto.MinStartDatetime != null && queryProjectDto.MaxStartDatetime < queryProjectDto.MinStartDatetime)
                    {
                        response.Success = false;
                        response.Message = "Invalid range for the queryable Start Date Time";
                        return response;
                    }
                    if (queryProjectDto.MaxUpdateDatetime != null && queryProjectDto.MinUpdateDatetime != null && queryProjectDto.MaxUpdateDatetime < queryProjectDto.MinUpdateDatetime)
                    {
                        response.Success = false;
                        response.Message = "Invalid range for the queryable Update Date Time";
                        return response;
                    }
                    if (queryProjectDto.MaxEndDatetime != null && queryProjectDto.MinEndDatetime != null && queryProjectDto.MaxEndDatetime < queryProjectDto.MinEndDatetime)
                    {
                        response.Success = false;
                        response.Message = "Invalid range for the queryable End Date Time";
                        return response;
                    }
                }
                //var result = await _unitOfWork.ProjectRepo.GetAllAsync();
                //var filteredResult = result.Where(p => p.Status == ProjectEnum.ONGOING || p.Status == ProjectEnum.HALTED && p.StartDatetime < p.EndDatetime);
                var filteredResult = await (await FilterProjects(user, queryProjectDto)).ToListAsync();
                var responseData = new List<ProjectDto>();
                foreach (var project in filteredResult)
                {
                    var monitor = await _unitOfWork.UserRepo.GetByIdAsync(project.MonitorId);
                    var creator = await _unitOfWork.UserRepo.GetByIdAsync(project.CreatorId);
                    var category = await _unitOfWork.ProjectCategoryRepo.GetListByProjectIdAsync(project.ProjectId);
                    var platform = await _unitOfWork.ProjectPlatformRepo.GetAllPlatformByProjectId(project.ProjectId);
                    var projectDto = new ProjectDto
                    {
                        ProjectId = project.ProjectId,
                        Thumbnail = project.Thumbnail ?? "Unknown",
                        Monitor = monitor?.Fullname ?? "Unknown",
                        CreatorId = project.CreatorId,
                        Creator = creator?.Fullname ?? "Unknown",
                        Title = project.Title,
                        Description = project.Description,
                        Status = project.Status,
                        MinimumAmount = project.MinimumAmount,
                        TotalAmount = project.TotalAmount,
                        StartDatetime = project.StartDatetime,
                        EndDatetime = project.EndDatetime,
                        Backers = await _unitOfWork.PledgeRepo.GetBackersByProjectIdAsync(project.ProjectId),
                        Categories = category.Select(c => new ViewCategory
                        {
                            CategoryId = c.CategoryId,
                            Name = c.Category.Name,
                            ParentCategoryId = c.Category.ParentCategoryId,
                            Description = c.Category.Description
                        }).ToList(),
                        Platforms = platform.Select(p => new PlatformDTO
                        {
                            PlatformId = p.PlatformId,
                            Name = p.Platform.Name,
                            Description = p.Platform.Description ?? "Null"
                        }).ToList()
                    };

                    responseData.Add(projectDto);
                }

                if (responseData.Any())
                {
                    response.Data = responseData;
                    response.Success = true;
                    response.Message = "Projects retrieved successfully.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "No projects found.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return response;
        }

        public async Task<ServiceResponse<ProjectDetailDto>> GetProjectById(int id)
        {
            var response = new ServiceResponse<ProjectDetailDto>();

            try
            {
                var project = await _unitOfWork.ProjectRepo.GetProjectById(id);
                if (project == null)
                {
                    response.Success = false;
                    response.Message = "Project not found.";
                    return response;
                }
                var monitor = await _unitOfWork.UserRepo.GetByIdAsync(project.MonitorId);
                var creator = await _unitOfWork.UserRepo.GetByIdAsync(project.CreatorId);
                var category = await _unitOfWork.ProjectCategoryRepo.GetListByProjectIdAsync(id);
                var platform = await _unitOfWork.ProjectPlatformRepo.GetAllPlatformByProjectId(id);
                var responseData = new ProjectDetailDto
                {
                    ProjectId = id,
                    Monitor = monitor?.Fullname ?? "unknown",
                    CreatorId = project.CreatorId,
                    Creator = creator?.Fullname ?? "unknown",
                    Thumbnail = project.Thumbnail ?? "unknown",
                    Story = project.Story ?? "No story found.",
                    Backers = await _unitOfWork.PledgeRepo.GetBackersByProjectIdAsync(project.ProjectId),
                    Title = project.Title,
                    Description = project.Description,
                    Status = project.Status,
                    MinimumAmount = project.MinimumAmount,
                    TotalAmount = project.TotalAmount,
                    StartDatetime = project.StartDatetime,
                    EndDatetime = project.EndDatetime,
                    Categories = category.Select(c => new ViewCategory
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Category.Name,
                        ParentCategoryId = c.Category.ParentCategoryId,
                        Description = c.Category.Description
                    }).ToList(),
                    Platforms = platform.Select(p => new PlatformDTO
                    {
                        PlatformId = p.PlatformId,
                        Name = p.Platform.Name,
                        Description = p.Platform.Description ?? "Null"
                    }).ToList()
                };

                response.Success = true;
                response.Message = "Get project successfully.";
                response.Data = responseData;

                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
                return response;
            }

            //return response;
        }

        public async Task<ServiceResponse<PaginationModel<ProjectDto>>> GetProjectsPaging(int pageNumber = 1, int pageSize = 5, User? user = null, QueryProjectDto? queryProjectDto = null)
        {
            var response = new ServiceResponse<PaginationModel<ProjectDto>>();

            try
            {
                //var (totalRecords, totalPages, projects) = await _unitOfWork.ProjectRepo.GetProjectsPaging(pageNumber, pageSize);
                var query = await FilterProjects(user, queryProjectDto);
                if (pageNumber <= 0)
                {
                    pageNumber = 1;
                }
                if (pageSize <= 0)
                {
                    pageSize = 5;
                }
                var totalRecords = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                var projects = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                var creator = await _unitOfWork.UserRepo.GetByIdAsync(projects.First().CreatorId);
                var monitor = await _unitOfWork.UserRepo.GetByIdAsync(projects.First().MonitorId);
                var projectDtos = new List<ProjectDto>();

                foreach (var project in projects)
                {
                    var projectDto = new ProjectDto
                    {
                        ProjectId = project.ProjectId,
                        Monitor = monitor?.Fullname ?? "unknown",
                        Creator = creator?.Fullname ?? "unknown",
                        Thumbnail = project.Thumbnail ?? "Unknown",
                        Backers = await _unitOfWork.PledgeRepo.GetBackersByProjectIdAsync(project.ProjectId),
                        Title = project.Title,
                        Description = project.Description,
                        Status = project.Status,
                        MinimumAmount = project.MinimumAmount,
                        TotalAmount = project.TotalAmount,
                        StartDatetime = project.StartDatetime,
                        EndDatetime = project.EndDatetime
                    };

                    projectDtos.Add(projectDto);
                }
                response.Data = new PaginationModel<ProjectDto>
                {
                    Page = pageNumber,
                    TotalPage = totalPages,
                    TotalRecords = totalRecords,
                    ListData = projectDtos
                };

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

        public async Task<ServiceResponse<UpdateProjectDto>> UpdateProject(int projectId, UpdateProjectDto updateProjectDto)
        {
            var response = new ServiceResponse<UpdateProjectDto>();

            try
            {
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdAsync(projectId);

                if (existingProject == null)
                {
                    response.Success = false;
                    response.Message = "Project not found.";
                    return response;
                }

                if (!string.IsNullOrWhiteSpace(updateProjectDto.Name))
                    existingProject.Title = updateProjectDto.Name;

                if (!string.IsNullOrWhiteSpace(updateProjectDto.Description))
                    existingProject.Description = updateProjectDto.Description;

                if (updateProjectDto.MinimumAmount > 0)
                {
                    existingProject.MinimumAmount = updateProjectDto.MinimumAmount;
                }

                if (updateProjectDto.StartDatetime != default)
                    existingProject.StartDatetime = updateProjectDto.StartDatetime;

                if (updateProjectDto.EndDatetime != default)
                    existingProject.EndDatetime = updateProjectDto.EndDatetime;

                if (updateProjectDto.StartDatetime >= updateProjectDto.EndDatetime)
                {
                    response.Success = false;
                    response.Message = "Start date must be earlier than end date.";
                    return response;
                }

                _mapper.Map(updateProjectDto, existingProject);

                await _unitOfWork.ProjectRepo.UpdateProject(projectId, existingProject);

                response.Data = updateProjectDto;
                response.Success = true;
                response.Message = "Project updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return response;
        }

        private async Task<string> CheckDescriptionAsync(string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return "Không có mô tả";
            var request = new { prompt = description };
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://gemini-ai-production.up.railway.app/GeminiAI/check-text", request);
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception ex)
            {
                return $"Lỗi khi gọi API: {ex.Message}";
            }
        }

        public async Task<ServiceResponse<ProjectThumbnailDto>> UpdateProjectThumbnail(int projectId, IFormFile thumbnail)
        {
            var response = new ServiceResponse<ProjectThumbnailDto>();
            var project = await _unitOfWork.ProjectRepo.GetByIdAsync(projectId);
            if (project == null)
            {
                response.Success = false;
                response.Message = "Project not found.";
                return response;
            }

            try
            {
                var uploadResult = new ImageUploadResult();

                if (thumbnail.Length > 0)
                {
                    using var stream = thumbnail.OpenReadStream();
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(thumbnail.FileName, stream),
                        Transformation = new Transformation().Crop("fill").Gravity("face")
                    };
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }

                if (uploadResult.Url == null)
                {
                    response.Success = false;
                    response.Message = "Could not upload image";
                    return response;
                }

                // Update the thumbnail URL in the database
                project.Thumbnail = uploadResult.Url.ToString();
                project.UpdateDatetime = DateTime.UtcNow;
                await _unitOfWork.ProjectRepo.UpdateProject(projectId, project);
                await _unitOfWork.SaveChangeAsync();

                var responseData = new ProjectThumbnailDto
                {
                    Thumbnail = project.Thumbnail
                };
                response.Data = responseData;
                response.Success = true;
                response.Message = "Thumbnail updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to update thumbnail: {ex.Message}";
            }

            return response;
        }
        public async Task<ServiceResponse<List<UserProjectsDto>>> GetProjectByUserIdAsync(int userId)
        {
            var response = new ServiceResponse<List<UserProjectsDto>>();
            try
            {
                var user = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                var projects = await _unitOfWork.ProjectRepo.GetProjectByUserIdAsync(userId);
                if (projects == null)
                {
                    response.Success = false;
                    response.Message = "Projects not found.";
                    return response;
                }
                projects.RemoveAll(p => p.Status == ProjectStatusEnum.DELETED);
                var projectList = _mapper.Map<List<UserProjectsDto>>(projects);
                response.Data = projectList;
                response.Success = true;
                response.Message = "Get projects by user id successfully.";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get project: {ex.Message}";
                return response;
            }
        }
        public async Task<ServiceResponse<ProjectStatusDTO>> StaffApproveAsync(int projectId, int userId, ProjectStatusEnum projectStatus, string reason)
        {
            var response = new ServiceResponse<ProjectStatusDTO>();

            try
            {
                var project = await _unitOfWork.ProjectRepo.GetProjectById(projectId);
                if (project == null)
                {
                    response.Success = false;
                    response.Message = "Project not found.";
                    return response;
                }

                var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                if (project.Status == projectStatus)
                {
                    response.Success = false;
                    response.Message = $"Project has been set as {projectStatus}";
                    return response;
                }

                if (project.Status == ProjectStatusEnum.DELETED)
                {
                    response.Success = false;
                    response.Message = "This project has been deleted.";
                    return response;
                }
                if (project.MonitorId != userId && user.Role != UserEnum.ADMIN)
                {
                    response.Success = false;
                    response.Message = "You are not authorized to approve or reject this project.";
                    return response;
                }
                var creator = await _unitOfWork.UserRepo.GetByIdAsync(project.CreatorId);
                if (creator == null)
                {
                    response.Success = false;
                    response.Message = "Creator not found.";
                    return response;
                }
                if (project.TransactionStatus != TransactionStatusEnum.REFUNDED && project.TransactionStatus != TransactionStatusEnum.TRANSFERED) 
                {
                    if (projectStatus == ProjectStatusEnum.VISIBLE)
                    {
                        project.TransactionStatus = TransactionStatusEnum.RECEIVING;
                    }
                    else
                    {
                        project.TransactionStatus = TransactionStatusEnum.PENDING;
                    }
                }
                project.Status = projectStatus;
                project.UpdateDatetime = DateTime.UtcNow;

                await _unitOfWork.ProjectRepo.UpdateProject(projectId, project);

                // Send email notification
                var emailSend = await EmailSender.SendProjectResponseEmail(creator.Email, project.Title ?? "[No Title | ID : " + project.ProjectId + "]", projectStatus, reason);
                if (!emailSend)
                {
                    response.Success = false;
                    response.Message = "Error when sending email notification.";
                    return response;
                }

                var responseData = new ProjectStatusDTO
                {
                    ProjectId = projectId,
                    Reason = reason,
                    Status = projectStatus.ToString()
                };
                response.Success = true;
                response.Data = responseData;
                response.Message = "Set Project Status Successfully.";

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return response;
        }

        public async Task<ServiceResponse<ProjectCategoryDto>> AddCategoryToProject(AddCategoryToProject addCategory)
        {
            var response = new ServiceResponse<ProjectCategoryDto>();

            try
            {
                var validationContext = new ValidationContext(addCategory);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(addCategory, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }


                var project = await _unitOfWork.ProjectRepo.GetByIdAsync(addCategory.ProjectId);
                if (project == null)
                {
                    response.Success = false;
                    response.Error = "Project not found!";
                    return response;
                }

                var category = await _unitOfWork.CategoryRepo.GetByIdAsync(addCategory.CategoryId);
                if (category == null)
                {
                    response.Success = false;
                    response.Error = "Category not found!";
                    return response;
                }

                var creator = await _unitOfWork.UserRepo.GetByIdAsync(project.CreatorId);

                var addCate = _mapper.Map<ProjectCategory>(addCategory);
                addCate.ProjectId = addCategory.ProjectId;

                await _unitOfWork.ProjectCategoryRepo.AddAsync(addCate);

                var responseData = new ProjectCategoryDto
                {
                    CategoryId = addCate.CategoryId,
                    ParentCategoryId = category.ParentCategoryId,
                    Name = category.Name,
                    CategoryDescription = category.Description,
                    ProjectId = addCategory.ProjectId,
                    Thumbnail = project.Thumbnail ?? "Null",
                    Monitor = project.Monitor?.Fullname ?? "Unknown",
                    CreatorId = project.CreatorId,
                    Creator = creator?.Fullname ?? "Unknown",
                    Backers = project.Pledges?.Count(pl => pl.ProjectId == project.ProjectId) ?? 0,
                    Title = project.Title,
                    ProjectDescription = project.Description,
                    Status = project.Status,
                    MinimumAmount = project.MinimumAmount,
                    TotalAmount = project.TotalAmount,
                    StartDatetime = project.StartDatetime,
                    EndDatetime = project.EndDatetime,
                };

                response.Data = responseData;
                response.Success = true;
                response.Message = "Category added to project successfully.";
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
