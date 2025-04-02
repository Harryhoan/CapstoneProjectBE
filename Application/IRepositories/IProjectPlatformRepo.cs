using Domain.Entities;

namespace Application.IRepositories
{
    public interface IProjectPlatformRepo : IGenericRepo<ProjectPlatform>
    {
        public Task<List<ProjectPlatform>> GetProjectPlatformsByPlatformId(int platformId);
        public Task<ProjectPlatform?> GetProjectPlatformByProjectIdAndPlatformId(int projectId, int platformId);
    }
}
