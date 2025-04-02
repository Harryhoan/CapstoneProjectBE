using Domain.Entities;

namespace Application.IRepositories
{
    public interface ICollaboratorRepo : IGenericRepo<Collaborator>
    {
        public Task<List<Collaborator>> GetCollaboratorsIncludeUserAndProject();
        public Task<Collaborator?> GetCollaboratorByUserIdAndProjectId(int userId, int projectId);
        public Task<Collaborator?> GetCollaboratorByUserIdAndProjectIdAsNoTracking(int userId, int projectId);
        public Task<List<Collaborator>> GetCollaboratorsByProjectId(int projectId);
        public Task<List<Collaborator>> GetCollaboratorsByProjectIdAsNoTracking(int projectId);
        public Task<List<Collaborator>> GetCollaboratorsByUserId(int userId);
        public Task<List<Collaborator>> GetCollaboratorsByUserIdAsNoTracking(int userId);
    }
}
