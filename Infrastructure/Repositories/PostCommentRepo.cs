using Application.IRepositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PostCommentRepo : GenericRepo<PostComment>, IPostCommentRepo
    {
        private readonly ApiContext _dbContext;

        public PostCommentRepo(ApiContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PostComment?> GetPostCommentByCommentId(int commentId)
        {
            return await _dbContext.PostComments.SingleOrDefaultAsync(pc => pc.CommentId == commentId);
        }
    }
}
