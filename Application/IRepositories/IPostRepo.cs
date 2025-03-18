using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories
{
    public interface IPostRepo : IGenericRepo<Post>
    {
        public Task<Post?> GetPostByPostIdAsNoTracking(int postId);
        public Task<List<Post>> GetPostsByProjectId(int projectId);
        public Task<List<Post>> GetPostsByUserId(int userId);

    }
}
