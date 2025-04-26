using Application.IRepositories;
using Application.ViewModels.ProjectDTO;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProjectPlatformRepo : GenericRepo<ProjectPlatform>, IProjectPlatformRepo
    {
        private readonly ApiContext _dbContext;

        public ProjectPlatformRepo(ApiContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProjectPlatform>> GetProjectPlatformsByPlatformId(int platformId)
        {
            return await _dbContext.ProjectPlatforms.Where(pp => pp.PlatformId == platformId).ToListAsync();
        }
        public async Task<ProjectPlatform?> GetProjectPlatformByProjectIdAndPlatformId(int projectId, int platformId)
        {
            return await _dbContext.ProjectPlatforms.SingleOrDefaultAsync(pp => pp.ProjectId == projectId && pp.PlatformId == platformId);
        }
        public async Task<List<ProjectPlatform>> GetAllProjectByPlatformId(int platformId)
        {
            return await _dbContext.ProjectPlatforms.Where(pp => pp.PlatformId == platformId).Include(pp => pp.Project).OrderBy(pp => pp.Project.Title).ToListAsync();
        }
    }
}
