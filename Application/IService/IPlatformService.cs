using Application.ServiceResponse;
using Application.ViewModels.PlatformDTO;
using Application.ViewModels.ProjectDTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IPlatformService
    {
        public Task<ServiceResponse<IEnumerable<ViewPlatformDto>>> GetAllPlatform();
        public Task<ServiceResponse<PaginationModel<ViewPlatformDto>>> GetPlatformsPaging(int pageNumber, int pageSize);
        public Task<ServiceResponse<ViewPlatformDto>> GetPlatformById(int id);
        public Task<ServiceResponse<ViewPlatformDto>> GetPlatformByProjectId(int projectId);
        public Task<ServiceResponse<int>> DeletePlatform(int id);
        public Task<ServiceResponse<CreatePlatformDto>> CreatePlatform(int userId, CreatePlatformDto createPlatformDto);
        public Task<ServiceResponse<UpdatePlatformDto>> UpdatePlatform(int platformId, UpdatePlatformDto updateProjectDto);
    }
}
