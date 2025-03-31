using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories
{
    public interface ICollaboratorRepo : IGenericRepo<Collaborator>
    {
        public Task<Collaborator?> GetCollaboratorByUserIdAndProjectId(int userId, int projectId);
        public Task<Collaborator?> GetCollaboratorByUserIdAndProjectIdAsNoTracking(int userId, int projectId);
    }
}
