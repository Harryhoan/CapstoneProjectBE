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
    internal class ProjectCommentRepo : GenericRepo<ProjectComment>, IProjectCommentRepo
    {
        public ProjectCommentRepo(ApiContext context) : base(context)
        {
        }
    }
}
