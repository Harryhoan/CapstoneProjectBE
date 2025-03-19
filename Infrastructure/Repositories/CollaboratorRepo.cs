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
    public class CollaboratorRepo : GenericRepo<Collaborator>, ICollaboratorRepo
    {
        private readonly ApiContext _dbContext;

        public CollaboratorRepo(ApiContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Collaborator?> GetCollaboratorByUserIdAndProjectId(int userId, int projectId)
        {
            return await _dbContext.Collaborators.SingleOrDefaultAsync(c => c.UserId == userId && c.ProjectId == projectId);
        }
    }
}
