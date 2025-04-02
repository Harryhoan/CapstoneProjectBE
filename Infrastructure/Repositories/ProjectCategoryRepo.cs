using Application.IRepositories;
using Domain;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class ProjectCategoryRepo : GenericRepo<ProjectCategory>, IProjectCategoryRepo
    {
        public ProjectCategoryRepo(ApiContext context) : base(context)
        {
        }
    }
}
