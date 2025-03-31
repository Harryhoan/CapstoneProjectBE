using Application.IRepositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using Domain;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class CollaboratorRepo : GenericRepo<Collaborator>, ICollaboratorRepo
    {
        private readonly ApiContext _dbContext;

        public CollaboratorRepo(ApiContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Collaborator>> GetCollaboratorsIncludeUserAndProject()
        {
            return await _dbContext.Collaborators.Include(c => c.User).Include(c => c.Project).ToListAsync();
        }

        public async Task<Collaborator?> GetCollaboratorByUserIdAndProjectId(int userId, int projectId)
        {
            return await _dbContext.Collaborators.Include(c => c.User).Include(c => c.Project).SingleOrDefaultAsync(c => c.UserId == userId && c.ProjectId == projectId);
        }
        public async Task<Collaborator?> GetCollaboratorByUserIdAndProjectIdAsNoTracking(int userId, int projectId)
        {
            return await _dbContext.Collaborators.AsNoTracking().SingleOrDefaultAsync(c => c.UserId == userId && c.ProjectId == projectId);
        }
        public async Task<List<Collaborator>> GetCollaboratorsByProjectId(int projectId)
        {
            return await _dbContext.Collaborators.Include(c => c.User).Where(c => c.ProjectId == projectId).ToListAsync();
        }
        public async Task<List<Collaborator>> GetCollaboratorsByProjectIdAsNoTracking(int projectId)
        {
            return await _dbContext.Collaborators.AsNoTracking().Where(c => c.ProjectId == projectId).ToListAsync();
        }
        public async Task<List<Collaborator>> GetCollaboratorsByUserId(int userId)
        {
            return await _dbContext.Collaborators.Include(c => c.Project).Where(c => c.UserId == userId).ToListAsync();
        }
        public async Task<List<Collaborator>> GetCollaboratorsByUserIdAsNoTracking(int userId)
        {
            return await _dbContext.Collaborators.AsNoTracking().Where(c => c.UserId == userId).ToListAsync();
        }
    }
}
