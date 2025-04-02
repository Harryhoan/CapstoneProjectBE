using Application.IRepositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class FAQRepo : GenericRepo<FAQ>, IFAQRepo
    {
        private readonly ApiContext _dbcontext;
        public FAQRepo(ApiContext context) : base(context)
        {
            _dbcontext = context;
        }
        public async Task<FAQ?> GetQuestionByQuestionAndProjectId(int projectId, string question)
        {
            return await _dbcontext.FAQs.FirstOrDefaultAsync(q => q.ProjectId == projectId || q.Question == question);
        }
        public async Task<List<FAQ>> GetAllQuestionsByProjectIdAsync(int projectId)
        {
            return await _dbcontext.FAQs.Where(q => q.ProjectId == projectId).ToListAsync();
        }
    }
}
