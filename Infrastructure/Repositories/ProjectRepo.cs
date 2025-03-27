using Application.IRepositories;
using Domain.Entities;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProjectRepo : GenericRepo<Project>, IProjectRepo
    {
        private readonly ApiContext _context;
        public ProjectRepo(ApiContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return 0;
            }
            _context.Projects.Remove(project);
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Project>> GetAll()
        {
            return await _context.Projects.ToListAsync();

        }

        public async Task<List<Project>> GetProjectByUserIdAsync(int userId)
        {
            return await _context.Projects.Where(p => p.CreatorId == userId).ToListAsync();
        }
        public async Task<Project?> GetProjectById(int id) => await _context.Projects.FindAsync(id);

        public async Task<(int, int, IEnumerable<Project>)> GetProjectsPaging(int pageNumber, int pageSize)
        {
            var totalRecord = await _context.Projects.CountAsync();
            var totalPage = (int)Math.Ceiling((double)totalRecord / pageSize);
            var projects = await _context.Projects
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (totalRecord, totalPage, projects);
        }

        public async Task<int> UpdateProject(int id, Project project)
        {
            var existingProject = await _context.Projects
                                 .FirstOrDefaultAsync(c => c.ProjectId == id);
            if (existingProject == null) return 0;
            project.ProjectId = id;
            _context.Entry(existingProject).CurrentValues.SetValues(project);
            _context.Entry(existingProject).State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }
    }
}
