using Application.ServiceResponse;
using Application.ViewModels.PlatformDTO;
using Application.ViewModels.ProjectDTO;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Application.IService
{
    public interface IPlatformService
    {
        public Task<ServiceResponse<int>> CreatePlatform(CreatePlatformDTO createPlatformDTO);
        public Task<ServiceResponse<PlatformDTO>> GetPlatformById(int platformId);
        public Task<ServiceResponse<List<PlatformDTO>>> GetPlatforms(string? query = null);
        public Task<ServiceResponse<PaginationModel<PlatformDTO>>> GetPaginatedPlatforms(string? query = null, int page = 1, int pageSize = 20);
        public Task<ServiceResponse<string>> UpdatePlatform(int platformId, CreatePlatformDTO createPlatformDTO);
        public Task<ServiceResponse<string>> RemovePlatform(int platformId);
        public Task<IActionResult?> CheckIfUserHasPermissionsByProjectId(int projectId, User? user = null);
        public Task<ServiceResponse<int>> CreateProjectPlatform(ProjectPlatformDTO projectPlatformDTO);
        public Task<ServiceResponse<int>> RemoveProjectPlatform(ProjectPlatformDTO projectPlatformDTO);
        public Task<ServiceResponse<List<PlatformDTO>>> GetAllPlatformAsync();
        public Task<ServiceResponse<List<ProjectDto>>> GetAllProjectByPlatformId(int platformId);
        public Task<ServiceResponse<List<PlatformDTO>>> GetAllPlatformByProjectId(int projectId);
    }
}
