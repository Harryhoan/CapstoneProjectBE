using Domain.Entities;

namespace Application.IRepositories
{
    public interface IReportRepo : IGenericRepo<Report>
    {
        public Task<List<Report>> GetReportsByUserIdAsync(int userId);
    }
}
