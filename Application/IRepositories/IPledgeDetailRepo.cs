using Domain.Entities;

namespace Application.IRepositories
{
    public interface IPledgeDetailRepo : IGenericRepo<PledgeDetail>
    {
        Task<List<PledgeDetail>> GetPledgeDetailByPledgeId(int pledgeId);
        Task<PledgeDetail?> GetByPaymentIdAsync(string paymentId);
    }
}
