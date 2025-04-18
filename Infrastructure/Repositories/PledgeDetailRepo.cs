using Application.IRepositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PledgeDetailRepo : GenericRepo<PledgeDetail>, IPledgeDetailRepo
    {
        private readonly ApiContext _dbContext;
        public PledgeDetailRepo(ApiContext context) : base(context)
        {
            _dbContext = context;
        }
        public async Task<List<PledgeDetail>> GetPledgeDetailByPledgeId(int pledgeId)
        {
            return await _dbContext.PledgeDetails.Where(pd => pd.PledgeId == pledgeId).ToListAsync();
        }
        public async Task<PledgeDetail?> GetByPaymentIdAsync(string paymentId)
        {
            return await _dbContext.Set<PledgeDetail>().FirstOrDefaultAsync(pd => pd.PaymentId == paymentId);
        }
    }
}
