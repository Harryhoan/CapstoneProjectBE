using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels.ProjectDTO;
using AutoMapper;
using Domain.Entities;
using System.Net.Http;
using System.Net.Http.Json;

namespace Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        public ProjectService(IUnitOfWork unitOfWork, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<ServiceResponse<CreateProjectDto>> CreateProject(CreateProjectDto createProjectDto)
        {
            var response = new ServiceResponse<CreateProjectDto>();

            try
            {
                var creatorExists = await _unitOfWork.UserRepo.Find(c => c.UserId == createProjectDto.CreatorId);
                if (!creatorExists)
                {
                    response.Success = false;
                    response.Message = "Invalid CreatorId.";
                    return response;
                }

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

                if (createProjectDto.TotalAmount < createProjectDto.MinimumAmount)
                {
                    response.Success = false;
                    response.Message = "Total amount must be greater than or equal to the minimum amount.";
                    return response;
                }

                var project = _mapper.Map<Project>(createProjectDto);

                await _unitOfWork.ProjectRepo.AddAsync(project);

                response.Data = createProjectDto;
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

        public async Task<ServiceResponse<IEnumerable<Project>>> GetAllProjects()
        {
            var response = new ServiceResponse<IEnumerable<Project>>();

            try
            {
                var result = await _unitOfWork.ProjectRepo.GetAllAsync();
                if (result != null && result.Any())
                {
                    response.Data = result;
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

        public async Task<ServiceResponse<Project>> GetProjectById(int id)
        {
            var response = new ServiceResponse<Project>();

            try
            {
                var project = await _unitOfWork.ProjectRepo.GetProjectById(id);

                if (project != null)
                {
                    response.Data = project;
                    response.Success = true;
                    response.Message = "Project retrieved successfully.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Project not found.";
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

        public async Task<ServiceResponse<PaginationModel<Project>>> GetProjectsPaging(int pageNumber, int pageSize)
        {
            var response = new ServiceResponse<PaginationModel<Project>>();

            try
            {
                var (totalRecords, totalPages, projects) = await _unitOfWork.ProjectRepo.GetProjectsPaging(pageNumber, pageSize);

                response.Data = new PaginationModel<Project>
                {
                    Page = pageNumber,
                    TotalPage = totalPages,
                    TotalRecords = totalRecords,
                    ListData = projects
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

                await _unitOfWork.ProjectRepo.UpdateProject(projectId,existingProject);

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
    }
}
