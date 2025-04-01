using Application.IRepositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return await _dbContext.GamePlatforms.Where(pp => pp.PlatformId == platformId).ToListAsync();
        }
        public async Task<ProjectPlatform?> GetProjectPlatformByProjectIdAndPlatformId(int projectId, int platformId)
        {
            return await _dbContext.GamePlatforms.SingleOrDefaultAsync(pp => pp.ProjectId == projectId && pp.PlatformId == platformId);
        }


    }
}
