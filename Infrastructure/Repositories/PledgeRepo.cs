using Application.IRepositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PledgeRepo : GenericRepo<Pledge>, IPledgeRepo
    {
        private readonly ApiContext _context;

        public PledgeRepo(ApiContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Pledge> GetPledgeByIdAsync(int id)
        {
            return await _context.Pledges.FindAsync(id);
        }
        public async Task<Pledge?> GetPledgeByUserIdAndProjectIdAsync(int userId, int projectId)
        {
            return await _context.Pledges
                .FirstOrDefaultAsync(p => p.UserId == userId && p.ProjectId == projectId);
        }
        public async Task<List<Pledge>> GetManyPledgeByUserIdAndProjectIdAsync(int userId, int projectId)
        {
            return await _context.Pledges
                .Where(p => p.UserId == userId && p.ProjectId == projectId).ToListAsync();
        }
        public async Task<List<Pledge>> GetPledgeByUserIdAsync(int userId)
        {
            return await _context.Pledges.Where(p => p.UserId == userId).ToListAsync();
        }
        public async Task<List<Pledge>> GetPledgeByProjectIdAsync(int projectId)
        {
            return await _context.Pledges.Where(p => p.ProjectId == projectId).ToListAsync();
        }
    }
}
