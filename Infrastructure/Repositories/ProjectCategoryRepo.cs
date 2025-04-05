using Application.IRepositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProjectCategoryRepo : GenericRepo<ProjectCategory>, IProjectCategoryRepo
    {
        public ProjectCategoryRepo(ApiContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProjectCategory>> GetListByProjectIdAsync(int projectId)
        {
            return await _context.ProjectCategories
                .Where(pc => pc.ProjectId == projectId)
                .Include(pc => pc.Category)
                .ToListAsync();
        }

    }
}
