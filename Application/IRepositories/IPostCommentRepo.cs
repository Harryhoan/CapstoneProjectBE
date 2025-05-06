using Domain.Entities;

namespace Application.IRepositories
{
    public interface IPostCommentRepo : IGenericRepo<PostComment>
    {
        public Task<PostComment?> GetPostCommentByCommentId(int commentId);
        public Task<List<PostComment>> GetPostCommentsByPostId(int postId);
    }
}
