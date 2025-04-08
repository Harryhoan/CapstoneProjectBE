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
        public async Task<List<ProjectCategory>> GetAllProjectByCategoryAsync(int categoryId)
        {
            var projectCategories = await _context.ProjectCategories
                .Where(pc => pc.CategoryId == categoryId)
                .Include(pc => pc.Project)
                .OrderBy(pc => pc.Project.Title) // Sort by Project Name
                .ToListAsync();

            // Return an empty list if no records are found
            return projectCategories ?? new List<ProjectCategory>();
        }
    }
}
