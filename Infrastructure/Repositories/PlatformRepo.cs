using Application.IRepositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PlatformRepo : GenericRepo<Platform>, IPlatformRepo
    {
        private readonly ApiContext _dbContext;

        public PlatformRepo(ApiContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Platform>> GetPlatformsByNameOrDescriptionAsNoTracking(string query)
        {
            query = query.Trim().ToLower();
            return await _dbContext.Platforms.AsNoTracking().Where(p => p.Name.Trim().ToLower().Contains(query) || (!string.IsNullOrEmpty(p.Description) && p.Description.Trim().ToLower().Contains(query))).ToListAsync();
        }

        public async Task<List<Platform>> GetPlatformsByProjectIdAsNoTracking(int projectId)
        {
            return await _dbContext.Platforms.AsNoTracking().Where(p => p.ProjectPlatforms.Any(pp => pp.ProjectId == projectId)).OrderBy(p => p.Name).ToListAsync();
        }

    }
}
