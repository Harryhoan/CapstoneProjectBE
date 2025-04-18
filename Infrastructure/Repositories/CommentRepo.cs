using Application.IRepositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CommentRepo : GenericRepo<Comment>, ICommentRepo
    {
        private readonly ApiContext _dbContext;

        public CommentRepo(ApiContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Comment>> GetCommentsWithCommentsByPostId(int postId)
        {
            var allComments = await _dbContext.PostComments
                .Where(pc => pc.PostId == postId)
                .Include(pc => pc.Comment)
                .ThenInclude(c => c.User)
                .Select(pc => pc.Comment)
                .ToListAsync();

            var parentComments = allComments.Where(c => c.ParentCommentId == null).ToList();

            foreach (var comment in parentComments)
            {
                AssignChildComments(comment, allComments);
            }

            return parentComments;
        }

        private static void AssignChildComments(Comment parentComment, List<Comment> allComments)
        {
            var childComments = allComments.Where(c => c.ParentCommentId == parentComment.CommentId).ToList();
            parentComment.Comments = childComments;

            foreach (var childComment in childComments)
            {
                AssignChildComments(childComment, allComments);
            }
        }
        public async Task<List<Comment>> GetCommentsWithCommentsByProjectId(int projectId)
        {
            var allComments = await _dbContext.ProjectComments
                .Where(pc => pc.ProjectId == projectId)
                .Include(pc => pc.Comment)
                .ThenInclude(c => c.User)
                .Select(pc => pc.Comment)
                .ToListAsync();

            var parentComments = allComments.Where(c => c.ParentCommentId == null).ToList();

            foreach (var comment in parentComments)
            {
                AssignChildComments(comment, allComments);
            }

            return parentComments;
        }
        public async Task<List<Comment>> GetCommentsByUserId(int userId)
        {
            return await _dbContext.Comments.Where(c => c.UserId == userId).ToListAsync();
        }

    }
}
