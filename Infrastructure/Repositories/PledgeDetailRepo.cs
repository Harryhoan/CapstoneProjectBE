using Application.IRepositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PledgeDetailRepo : GenericRepo<PledgeDetail>, IPledgeDetailRepo
    {
        private readonly ApiContext _context;
        public PledgeDetailRepo(ApiContext context) : base(context)
        {
            _context = context;
        }
        public async Task<PledgeDetail> GetByPledgeIdAsync(int id, string paymentId)
        {
            return await _context.PledgeDetails.FirstOrDefaultAsync(pd => pd.PledgeId == id && pd.PaymentId == paymentId);
        }
    }
}
