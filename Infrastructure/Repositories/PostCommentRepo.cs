using Application.IRepositories;
using Domain.Entities;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PostCommentRepo : GenericRepo<PostComment>, IPostCommentRepo
    {
        public PostCommentRepo(ApiContext context) : base(context)
        {
        }
    }
}
