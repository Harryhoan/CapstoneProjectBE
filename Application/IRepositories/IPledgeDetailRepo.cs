using Domain.Entities;

namespace Application.IRepositories
{
    public interface IPledgeDetailRepo : IGenericRepo<PledgeDetail>
    {
        Task<List<PledgeDetail>> GetPledgeDetailByPledgeId(int pledgeId);
    }
}
