using Domain.Entities;

namespace Application.IRepositories
{
    public interface IPledgeRepo : IGenericRepo<Pledge>
    {
        Task<Pledge?> GetPledgeByUserIdAndProjectIdAsync(int userId, int projectId);
        Task<List<Pledge>> GetManyPledgeByUserIdAndProjectIdAsync(int userId, int projectId);
        Task<List<Pledge>> GetPledgeByUserIdAsync(int userId);
        Task<List<Pledge>> GetPledgesByProjectIdAsync(int projectId);
        Task<int> GetBackersByProjectIdAsync(int projectId);
    }
}
