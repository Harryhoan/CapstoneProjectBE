using Application.IRepositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    internal class ProjectCommentRepo : GenericRepo<ProjectComment>, IProjectCommentRepo
    {
        private readonly ApiContext _dbContext;

        public ProjectCommentRepo(ApiContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProjectComment?> GetProjectCommentByCommentId(int commentId)
        {
            return await _dbContext.ProjectComments.SingleOrDefaultAsync(pc => pc.CommentId == commentId);
        }
    }
}
