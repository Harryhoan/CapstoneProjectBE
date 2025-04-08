using Application.IRepositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RewardRepo : GenericRepo<Reward>, IRewardRepo
    {
        private readonly ApiContext _dbContext;
        public RewardRepo(ApiContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task<List<Reward>> GetRewardsByProjectIdAsync(int projectId)
        {
            return await _dbContext.Rewards.Where(r => r.ProjectId == projectId).ToListAsync();
        }
    }
}
