using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels;
using Application.ViewModels.ProjectDTO;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Threading;

namespace Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly Cloudinary _cloudinary;
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

        public async Task<ServiceResponse<ProjectDto>> CreateProject(int userId, CreateProjectDto createProjectDto)
        {
            var response = new ServiceResponse<ProjectDto>();
            var user = await _unitOfWork.UserRepo.GetAllAsync();
            try
            {
                if (string.IsNullOrWhiteSpace(createProjectDto.Title))
                {
                    response.Success = false;
                    response.Message = "Project title is required.";
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


                var staffUsers = user.Where(u => u.Role == "Staff").ToList();
                if (!staffUsers.Any())
                {
                    response.Success = false;
                    response.Message = "No staff users available to assign as monitor.";
                    return response;
                }
                var random = new Random();
                var randomStaff = staffUsers[random.Next(staffUsers.Count)];


                project.MonitorId = randomStaff.UserId;
                project.CreatorId = userId;
                project.MinimumAmount = 0;
                project.Status = ProjectEnum.INVISIBLE;
                project.UpdateDatetime = createProjectDto.StartDatetime;
                await _unitOfWork.ProjectRepo.AddAsync(project);
                var responseData = new ProjectDto
                {
                    ProjectId = project.ProjectId,
                    Monitor = randomStaff.Fullname,
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

        public async Task<ServiceResponse<IEnumerable<ProjectDto>>> GetAllProjects()
        {
            var response = new ServiceResponse<IEnumerable<ProjectDto>>();

            try
            {
                var result = await _unitOfWork.ProjectRepo.GetAllAsync();
                var filteredResult = result.Where(p => p.Status == ProjectEnum.ONGOING && p.StartDatetime < p.EndDatetime); // Filter projects by status and date range

                var responseData = new List<ProjectDto>();
                foreach (var project in filteredResult)
                {
                    var monitor = await _unitOfWork.UserRepo.GetByIdAsync(project.MonitorId);
                    var creator = await _unitOfWork.UserRepo.GetByIdAsync(project.CreatorId);

                    var projectDto = new ProjectDto
                    {
                        ProjectId = project.ProjectId,
                        Thumbnail = project.Thumbnail,
                        Monitor = monitor?.Fullname ?? "Unknown",
                        Creator = creator?.Fullname ?? "Unknown",
                        Title = project.Title,
                        Description = project.Description,
                        Status = project.Status,
                        MinimumAmount = project.MinimumAmount,
                        TotalAmount = project.TotalAmount,
                        StartDatetime = project.StartDatetime,
                        EndDatetime = project.EndDatetime,
                        Backers = project.Pledges.Count(pl => pl.ProjectId == project.ProjectId) // Calculate the total number of backers with valid ProjectId
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

        public async Task<ServiceResponse<ProjectDto>> GetProjectById(int id)
        {
            var response = new ServiceResponse<ProjectDto>();

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

                var responseData = new ProjectDto
                {
                    ProjectId = id,
                    Monitor = monitor?.Fullname ?? "unknown",
                    Creator = creator?.Fullname ?? "unknown",
                    Thumbnail = project.Thumbnail,
                    Backers = project.Pledges.Count(pl => pl.ProjectId == id),
                    Title = project.Title,
                    Description = project.Description,
                    Status = project.Status,
                    MinimumAmount = project.MinimumAmount,
                    TotalAmount = project.TotalAmount,
                    StartDatetime = project.StartDatetime,
                    EndDatetime = project.EndDatetime
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

        public async Task<ServiceResponse<PaginationModel<ProjectDto>>> GetProjectsPaging(int pageNumber, int pageSize)
        {
            var response = new ServiceResponse<PaginationModel<ProjectDto>>();

            try
            {
                var (totalRecords, totalPages, projects) = await _unitOfWork.ProjectRepo.GetProjectsPaging(pageNumber, pageSize);
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
                        Thumbnail = project.Thumbnail,
                        Backers = project.Pledges.Count(pl => pl.ProjectId == project.ProjectId),
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

                if (string.IsNullOrWhiteSpace(updateProjectDto.Name))
                {
                    response.Success = false;
                    response.Message = "Project name is required.";
                    return response;
                }

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
            if ( project == null)
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
                    using (var stream = thumbnail.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(thumbnail.FileName, stream),
                            Transformation = new Transformation().Crop("fill").Gravity("face")
                        };
                        uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    }
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
                    thumbnail = project.Thumbnail
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

        public async Task<ServiceResponse<string>> StaffApproveAsync(int projectId, int userId, bool isApproved, string reason)
        {
            var response = new ServiceResponse<string>();

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

                if (project.MonitorId != userId && user.Role != "Admin")
                {
                    response.Success = false;
                    response.Message = "You are not authorized to approve or reject this project.";
                    return response;
                }

                project.Status = isApproved ? ProjectEnum.ONGOING : ProjectEnum.HALTED;
                //project.ApprovalReason = reason;
                project.UpdateDatetime = DateTime.UtcNow;

                await _unitOfWork.ProjectRepo.UpdateProject(projectId, project);
                await _unitOfWork.SaveChangeAsync();

                response.Success = true;
                response.Message = isApproved ? "Project approved successfully." : "Project rejected successfully.";
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
