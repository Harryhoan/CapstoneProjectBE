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
    public class ReportRepo : GenericRepo<Report>, IReportRepo
    {
        private readonly ApiContext _dbContext;
        public ReportRepo(ApiContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Report>> GetReportsByUserIdAsync(int userId)
        {
            return await _dbContext.Reports.Where(r => r.UserId == userId).ToListAsync();
        }
    }
}
