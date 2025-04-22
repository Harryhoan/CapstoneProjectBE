using Domain.Entities;

namespace Application.IRepositories
{
    public interface IPledgeRepo : IGenericRepo<Pledge>
    {
        Task<Pledge?> GetPledgeByUserIdAndProjectIdAsync(int userId, int projectId);
        Task<List<Pledge>> GetManyPledgesByUserIdAndProjectIdAsync(int userId, int projectId);
        Task<List<Pledge>> GetPledgesByUserIdAsync(int userId);
        Task<List<Pledge>> GetPledgesByProjectIdAsync(int projectId);
        Task<int> GetBackersByProjectIdAsync(int projectId);
    }
}
