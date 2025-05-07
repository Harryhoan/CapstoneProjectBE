using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels.FaqDTO;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class FAQService : IFAQService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FAQService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<FAQ>> AddFaq(int userId, int projectId, FaqDto createFAQ)
        {
            var response = new ServiceResponse<FAQ>();

            try
            {
                var userExists = await _unitOfWork.UserRepo.Find(u => u.UserId == userId);
                if (!userExists)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }
                var projectExists = await _unitOfWork.ProjectRepo.Find(p => p.ProjectId == projectId);
                if (!projectExists)
                {
                    response.Success = false;
                    response.Message = "Project not found.";
                    return response;
                }
                var existFaq = await _unitOfWork.FAQRepo.GetAllQuestionsByProjectIdAsync(projectId);
                foreach (var item in existFaq)
                {
                    if (item.Question == createFAQ.Question)
                    {
                        response.Success = false;
                        response.Message = "This question existed.";
                        return response;
                    }
                }

                var newFAQ = _mapper.Map<FAQ>(createFAQ);

                newFAQ.ProjectId = projectId;
                newFAQ.CreatedDatetime = DateTime.UtcNow.AddHours(7);
                newFAQ.UpdatedDatetime = DateTime.UtcNow.AddHours(7);
                await _unitOfWork.FAQRepo.AddAsync(newFAQ);

                response.Data = newFAQ;
                response.Success = true;
                response.Message = "FAQ created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return response;
        }

        public async Task<ServiceResponse<string>> DeleteFAQ(int userId, int projectId, string question)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }
                var faq = await _unitOfWork.FAQRepo.GetQuestionByQuestionAndProjectId(projectId, question);
                if (faq == null)
                {
                    response.Success = false;
                    response.Message = "Faq not found.";
                    return response;
                }

                await _unitOfWork.FAQRepo.RemoveAsync(faq);

                response.Success = true;
                response.Message = "Delete Faq succesfully.";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to delete Faq: {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<List<FaqDto>>> GetFaqByProjectId(int userId, int projectId)
        {
            var response = new ServiceResponse<List<FaqDto>>();
            try
            {
                var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }
                var faq = await _unitOfWork.FAQRepo.GetAllQuestionsByProjectIdAsync(projectId);
                if (faq == null)
                {
                    response.Success = false;
                    response.Message = "Question not found.";
                    return response;
                }

                var FaqData = _mapper.Map<List<FaqDto>>(faq);
                response.Success = true;
                response.Message = "Get Faq by ProjectId successfully.";
                response.Data = FaqData;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get Faq: {ex.Message}";
                return response;
            }
        }
        public async Task<ServiceResponse<List<FaqDto>>> GetAllFaqByProjectIdAsync(int projectId)
        {
            var response = new ServiceResponse<List<FaqDto>>();
            try
            {
                var faqs = await _unitOfWork.FAQRepo.GetAllQuestionsByProjectIdAsync(projectId);
                if (faqs == null)
                {
                    response.Success = true;
                    response.Message = "No Faq found for this project.";
                    return response;
                }

                var responseData = _mapper.Map<List<FaqDto>>(faqs);

                response.Success = true;
                response.Message = "Get all faq successfully.";
                response.Data = responseData;
                return response;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get Faq: {ex.Message}";
                return response;
            }
        }
        //public async Task<ServiceResponse<IEnumerable<FAQ>>> GetAllFAQ()
        //{
        //    var response = new ServiceResponse<IEnumerable<FAQ>>();

        //    try
        //    {
        //        var result = await _unitOfWork.FAQRepo.GetAllAsync();
        //        if (result != null && result.Any())
        //        {
        //            response.Data = result;
        //            response.Success = true;
        //            response.Message = "FAQs retrieved successfully.";
        //        }
        //        else
        //        {
        //            response.Success = false;
        //            response.Message = "No FAQs found.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Success = false;
        //        response.Error = ex.Message;
        //        response.ErrorMessages = new List<string> { ex.ToString() };
        //    }
        //    return response;
        //}

        public async Task<ServiceResponse<FaqDto>> UpdateFaq(int userId, int projectId, string Question, FaqDto UpdateFaq)
        {
            var response = new ServiceResponse<FaqDto>();
            try
            {
                var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                var project = await _unitOfWork.ProjectRepo.GetByIdAsync(projectId);
                if (project == null)
                {
                    response.Success = false;
                    response.Message = "Project not found.";
                    return response;
                }

                var faq = await _unitOfWork.FAQRepo.GetQuestionByQuestionAndProjectId(projectId, Question);
                if (faq == null)
                {
                    response.Success = false;
                    response.Message = "Faq not found.";
                    return response;
                }

                // Update the existing FAQ instead of deleting and recreating
                faq.Question = UpdateFaq.Question;
                faq.Answer = UpdateFaq.Answer;
                faq.UpdatedDatetime = DateTime.UtcNow.AddHours(7);

                await _unitOfWork.FAQRepo.UpdateAsync(faq);

                response.Success = true;
                response.Message = "Faq updated successfully.";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
                return response;
            }
        }


    }
}
