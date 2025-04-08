using Domain.Entities;

namespace Application.IRepositories
{
    public interface IRewardRepo : IGenericRepo<Reward>
    {
        public Task<List<Reward>> GetRewardsByProjectIdAsync(int projectId);
    }
}
