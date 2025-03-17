using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories
{
    public interface IPledgeDetailRepo : IGenericRepo<PledgeDetail>
    {
        Task<PledgeDetail> GetByPledgeIdAsync(int id, string paymentId);
    }
}
