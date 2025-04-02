using Domain.Entities;

namespace Application.IRepositories
{
    public interface IProjectRepo : IGenericRepo<Project>
    {
        Task<IEnumerable<Project>> GetAll();
        Task<(int, int, IEnumerable<Project>)> GetProjectsPaging(int pageNumber, int pageSize);
        Task<Project?> GetProjectById(int id);
        Task<int> DeleteProject(int id);
        Task<int> UpdateProject(int id, Project project);
        Task<List<Project>> GetProjectByUserIdAsync(int userId);
        Task<List<Project>> GetAllProjectByMonitorIdAsync(int userId);
    }
}
