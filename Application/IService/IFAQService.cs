using Application.ServiceResponse;
using Application.ViewModels.FaqDTO;
using Domain.Entities;

namespace Application.IService
{
    public interface IFAQService
    {
        public Task<ServiceResponse<string>> DeleteFAQ(int userId, int projectId, string question);
        public Task<ServiceResponse<List<FaqDto>>> GetFaqByProjectId(int userId, int projectId);
        public Task<ServiceResponse<FAQ>> AddFaq(int userId, int projectId, FaqDto createFaq);
        public Task<ServiceResponse<FaqDto>> UpdateFaq(int userId, int projectId, string Question, FaqDto UpdateFaq);
        public Task<ServiceResponse<List<FaqDto>>> GetAllFaqByProjectIdAsync(int projectId);
    }
}
