using Application.IRepositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PostRepo : GenericRepo<Post>, IPostRepo
    {
        private readonly ApiContext _dbContext;

        public PostRepo(ApiContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Post?> GetPostByPostIdAsNoTracking(int postId)
        {
            return await _dbContext.Posts.AsNoTracking().SingleOrDefaultAsync(p => p.PostId == postId);
        }
        public async Task<List<Post>> GetPostsByProjectId(int projectId)
        {
            return await _dbContext.Posts.Include(p => p.User).Where(p => p.ProjectId == projectId).ToListAsync();
        }
        public async Task<List<Post>> GetPostsByUserId(int userId)
        {
            return await _dbContext.Posts.Include(p => p.User).Where(p => p.UserId == userId).ToListAsync();
        }
    }
}
