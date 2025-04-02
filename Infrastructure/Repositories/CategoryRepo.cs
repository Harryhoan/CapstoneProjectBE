using Application.IRepositories;
using Domain;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class CategoryRepo : GenericRepo<Category>, ICategoryRepo
    {
        public CategoryRepo(ApiContext context) : base(context)
        {
        }
    }
}
