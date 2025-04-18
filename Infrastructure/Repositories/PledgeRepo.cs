using Application.IRepositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PledgeRepo : GenericRepo<Pledge>, IPledgeRepo
    {
        private readonly ApiContext _dbContext;

        public PledgeRepo(ApiContext context) : base(context)
        {
            _dbContext = context;
        }
        public async Task<int> GetBackersByProjectIdAsync(int projectId)
        {
            return await _dbContext.Pledges.Where(p => p.ProjectId == projectId).CountAsync();
        }

        public async Task<Pledge?> GetPledgeByUserIdAndProjectIdAsync(int userId, int projectId)
        {
            return await _dbContext.Pledges
                .FirstOrDefaultAsync(p => p.UserId == userId && p.ProjectId == projectId);
        }
        public async Task<List<Pledge>> GetManyPledgeByUserIdAndProjectIdAsync(int userId, int projectId)
        {
            return await _dbContext.Pledges
                .Where(p => p.UserId == userId && p.ProjectId == projectId).ToListAsync();
        }
        public async Task<List<Pledge>> GetPledgeByUserIdAsync(int userId)
        {
            return await _dbContext.Pledges.Where(p => p.UserId == userId).ToListAsync();
        }
        public async Task<List<Pledge>> GetPledgeByProjectIdAsync(int projectId)
        {
            return await _dbContext.Pledges.Where(p => p.ProjectId == projectId).ToListAsync();
        }
    }
}
