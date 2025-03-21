using Application.ServiceResponse;
using Application.ViewModels.PledgeDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IPledgeService
    {
        public Task<ServiceResponse<IEnumerable<PledgeDto>>> GetAllPledgeByAdmin();
        public Task<ServiceResponse<PledgeDto>> GetPledgeById(int pledgeId);
        public Task<ServiceResponse<List<PledgeDto>>> GetPledgeByUserId(int userId);
    }
}
