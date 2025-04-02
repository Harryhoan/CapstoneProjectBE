using Application.ServiceResponse;
using Application.ViewModels.ProjectDTO;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.IService
{
    public interface IProjectService
    {
        public Task<ServiceResponse<IEnumerable<ProjectDto>>> GetAllProjects();
        public Task<ServiceResponse<PaginationModel<ProjectDto>>> GetProjectsPaging(int pageNumber, int pageSize);
        public Task<ServiceResponse<ProjectDetailDto>> GetProjectById(int id);
        public Task<ServiceResponse<int>> DeleteProject(int id);
        public Task<ServiceResponse<ProjectDto>> CreateProject(int userId, CreateProjectDto createProjectDto);
        public Task<ServiceResponse<UpdateProjectDto>> UpdateProject(int projectId, UpdateProjectDto updateProjectDto);
        public Task<ServiceResponse<ProjectThumbnailDto>> UpdateProjectThumbnail(int projectId, IFormFile thumbnail);
        public Task<ServiceResponse<ProjectStatusDTO>> StaffApproveAsync(int projectId, int userId, ProjectEnum projectStatus, string reason);
        public Task<ServiceResponse<string>> UpdateProjectStoryAsync(int userId, int projectId, string story);
        public Task<ServiceResponse<List<UserProjectsDto>>> GetProjectByUserIdAsync(int userId);
        public Task<ServiceResponse<List<ProjectDto>>> GetAllProjectByAdminAsync(int userId);
        public Task<ServiceResponse<ProjectCategoryDto>> AddCategoryToProject(AddCategoryToProject addCategory);
    }
}
