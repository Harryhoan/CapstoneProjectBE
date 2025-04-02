using Domain.Entities;

namespace Application.IRepositories
{
    public interface IProjectCommentRepo : IGenericRepo<ProjectComment>
    {
        public Task<ProjectComment?> GetProjectCommentByCommentId(int commentId);
    }
}
