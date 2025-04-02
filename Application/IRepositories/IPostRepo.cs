using Domain.Entities;

namespace Application.IRepositories
{
    public interface IPostRepo : IGenericRepo<Post>
    {
        public Task<Post?> GetPostByPostIdAsNoTracking(int postId);
        public Task<List<Post>> GetPostsByProjectId(int projectId);
        public Task<List<Post>> GetPostsByUserId(int userId);

    }
}
