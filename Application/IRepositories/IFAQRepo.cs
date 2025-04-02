using Domain.Entities;

namespace Application.IRepositories
{
    public interface IFAQRepo : IGenericRepo<FAQ>
    {
        Task<FAQ?> GetQuestionByQuestionAndProjectId(int projectId, string question);
        Task<List<FAQ>> GetAllQuestionsByProjectIdAsync(int projectId);
    }
}
