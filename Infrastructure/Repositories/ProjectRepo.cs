using Application.IRepositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProjectRepo : GenericRepo<Project>, IProjectRepo
    {
        private readonly ApiContext _dbContext;
        public ProjectRepo(ApiContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task<int> DeleteProject(int id)
        {
            var project = await _dbContext.Projects.FindAsync(id);
            if (project == null)
            {
                return 0;
            }
            _dbContext.Projects.Remove(project);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Project>> GetAll()
        {
            return await _dbContext.Projects.ToListAsync();

        }

        public async Task<List<Project>> GetAllProjectByMonitorIdAsync(int userId)
        {
            return await _dbContext.Projects.Where(p => p.MonitorId == userId).ToListAsync();
        }
        public async Task<List<Project>> GetProjectByUserIdAsync(int userId)
        {
            return await _dbContext.Projects.Where(p => p.CreatorId == userId).ToListAsync();
        }
        public async Task<Project?> GetProjectById(int id) => await _dbContext.Projects.FindAsync(id);

        public async Task<(int, int, IEnumerable<Project>)> GetProjectsPaging(int pageNumber, int pageSize)
        {
            var totalRecord = await _dbContext.Projects.CountAsync();
            var totalPage = (int)Math.Ceiling((double)totalRecord / pageSize);
            var projects = await _dbContext.Projects
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (totalRecord, totalPage, projects);
        }

        public async Task<int> UpdateProject(int id, Project project)
        {
            var existingProject = await _dbContext.Projects
                                 .FirstOrDefaultAsync(c => c.ProjectId == id);
            if (existingProject == null) return 0;
            project.ProjectId = id;
            _dbContext.Entry(existingProject).CurrentValues.SetValues(project);
            _dbContext.Entry(existingProject).State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync();
        }
    }
}
