using Application.ServiceResponse;
using Application.ViewModels.FaqDTO;
using Domain.Entities;

namespace Application.IService
{
    public interface IFAQService
    {
        public Task<ServiceResponse<string>> DeleteFAQ(int userId, int projectId, string question);
        public Task<ServiceResponse<List<ViewFaqDto>>> GetFaqByProjectId(int userId, int projectId);
        public Task<ServiceResponse<ViewFaqDto>> AddFaq(int userId, int projectId, FaqDto createFaq);
        public Task<ServiceResponse<ViewFaqDto>> UpdateFaq(int userId, int projectId, string Question, FaqDto updateFaq);
        public Task<ServiceResponse<List<ViewFaqDto>>> GetAllFaqByProjectIdAsync(int projectId);
    }
}
