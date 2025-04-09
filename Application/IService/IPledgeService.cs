using Application.ServiceResponse;
using Application.ViewModels.PledgeDTO;
using Domain.Entities;

namespace Application.IService
{
    public interface IPledgeService
    {
        public Task<ServiceResponse<IEnumerable<PledgeDto>>> GetAllPledgeByAdmin();
        public Task<ServiceResponse<PledgeDto>> GetPledgeById(int pledgeId);
        public Task<ServiceResponse<List<PledgeDto>>> GetPledgeByUserId(int userId);
        public Task<ServiceResponse<string>> ExportPledgeToExcelByProjectId(int projectId);
        public Task<ServiceResponse<List<ProjectBackerDto>>> GetBackerByProjectId(int projectId);
    }
}
