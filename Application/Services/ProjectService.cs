using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels.ProjectDTO;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProjectService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<CreateProjectDto>> CreateProject(CreateProjectDto createProjectDto)
        {
            var response = new ServiceResponse<CreateProjectDto>();

            try
            {
                if (string.IsNullOrWhiteSpace(createProjectDto.Title))
                {
                    response.Success = false;
                    response.Message = "Project title is required.";
                    return response;
                }

                if (createProjectDto.StartDatetime >= createProjectDto.EndDatetime)
                {
                    response.Success = false;
                    response.Message = "Start date must be earlier than end date.";
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
    }
}
