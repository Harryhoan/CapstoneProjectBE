using Application.IRepositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PledgeDetailRepo : GenericRepo<PledgeDetail>, IPledgeDetailRepo
    {
        private readonly ApiContext _context;
        public PledgeDetailRepo(ApiContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<PledgeDetail>> GetPledgeDetailByPledgeId(int pledgeId)
        {
            return await _context.PledgeDetails.Where(pd => pd.PledgeId == pledgeId).ToListAsync();
        }
        public async Task<PledgeDetail?> GetByPaymentIdAsync(string paymentId)
        {
            return await _context.Set<PledgeDetail>().FirstOrDefaultAsync(pd => pd.PaymentId == paymentId);
        }
    }
}
