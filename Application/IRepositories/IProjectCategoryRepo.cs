using Domain.Entities;

namespace Application.IRepositories
{
    public interface IProjectCategoryRepo : IGenericRepo<ProjectCategory>
    {
        Task<IEnumerable<ProjectCategory>> GetListByProjectIdAsync(int projectId);
    }
}
