using Domain.Entities;

namespace Application.IRepositories
{
    public interface IPlatformRepo : IGenericRepo<Platform>
    {
        public Task<List<Platform>> GetPlatformsByNameOrDescriptionAsNoTracking(string query);
        public Task<List<Platform>> GetPlatformsByProjectIdAsNoTracking(int projectId);
    }
}
