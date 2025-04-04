using Domain.Entities;

namespace Application.IRepositories
{
    public interface IPledgeRepo : IGenericRepo<Pledge>
    {
        Task<Pledge> GetPledgeByIdAsync(int id);
        Task<Pledge?> GetPledgeByUserIdAndProjectIdAsync(int userId, int projectId);
        Task<List<Pledge>> GetManyPledgeByUserIdAndProjectIdAsync(int userId, int projectId);
        Task<List<Pledge>> GetPledgeByUserIdAsync(int userId);
        Task<List<Pledge>> GetPledgeByProjectIdAsync(int projectId);
        Task<int> GetBackersByProjectIdAsync(int projectId);
    }
}
