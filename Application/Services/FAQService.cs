using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels.FaqDTO;
using AutoMapper;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

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
        public async Task<ServiceResponse<ViewFaqDto>> AddFaq(int userId, int projectId, FaqDto createFAQ)
        {
            var response = new ServiceResponse<ViewFaqDto>();

            try
            {
                var validationContext = new ValidationContext(createFAQ);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(createFAQ, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

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
                var createQuestion = FormatUtils.TrimSpacesPreserveSingle(createFAQ.Question.Trim().ToLower());
                foreach (var item in existFaq)
                {
                    var itemQuestion = FormatUtils.TrimSpacesPreserveSingle(item.Question.Trim().ToLower());
                    if (createQuestion.Equals(itemQuestion, StringComparison.OrdinalIgnoreCase) /*|| createQuestion.Contains(itemQuestion, StringComparison.OrdinalIgnoreCase) || itemQuestion.Contains(createQuestion, StringComparison.OrdinalIgnoreCase)*/)
                    {
                        response.Success = false;
                        response.Message = "This question has already existed and been answered.";
                        return response;
                    }
                }
                createFAQ.Question = FormatUtils.TrimSpacesPreserveSingle(createFAQ.Question);
                var newFAQ = _mapper.Map<FAQ>(createFAQ);

                newFAQ.ProjectId = projectId;
                newFAQ.CreatedDatetime = DateTime.UtcNow.AddHours(7);
                newFAQ.UpdatedDatetime = DateTime.UtcNow.AddHours(7);
                await _unitOfWork.FAQRepo.AddAsync(newFAQ);

                response.Data = _mapper.Map<ViewFaqDto>(newFAQ);
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
                var user = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
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

        public async Task<ServiceResponse<List<ViewFaqDto>>> GetFaqByProjectId(int userId, int projectId)
        {
            var response = new ServiceResponse<List<ViewFaqDto>>();
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

                var FaqData = _mapper.Map<List<ViewFaqDto>>(faq);
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
        public async Task<ServiceResponse<List<ViewFaqDto>>> GetAllFaqByProjectIdAsync(int projectId)
        {
            var response = new ServiceResponse<List<ViewFaqDto>>();
            try
            {
                var faqs = await _unitOfWork.FAQRepo.GetAllQuestionsByProjectIdAsync(projectId);
                if (faqs == null)
                {
                    response.Success = true;
                    response.Message = "No Faq found for this project.";
                    return response;
                }

                var responseData = _mapper.Map<List<ViewFaqDto>>(faqs);

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

        public async Task<ServiceResponse<ViewFaqDto>> UpdateFaq(int userId, int projectId, string Question, FaqDto updateFaq)
        {
            var response = new ServiceResponse<ViewFaqDto>();
            try
            {
                var validationContext = new ValidationContext(updateFaq);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(updateFaq, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                var project = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
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

                var existFaq = await _unitOfWork.FAQRepo.GetAllQuestionsByProjectIdAsync(projectId);
                var updateQuestion = FormatUtils.TrimSpacesPreserveSingle(updateFaq.Question.Trim().ToLower());
                foreach (var item in existFaq)
                {
                    var itemQuestion = FormatUtils.TrimSpacesPreserveSingle(item.Question.Trim().ToLower());
                    if (updateQuestion.Equals(itemQuestion, StringComparison.OrdinalIgnoreCase) /*|| updateQuestion.Contains(itemQuestion, StringComparison.OrdinalIgnoreCase) || itemQuestion.Contains(updateQuestion, StringComparison.OrdinalIgnoreCase)*/)
                    {
                        response.Success = false;
                        response.Message = "This question has already existed and been answered.";
                        return response;
                    }
                }

                faq.Question = FormatUtils.TrimSpacesPreserveSingle(updateFaq.Question);
                faq.Answer = updateFaq.Answer;
                faq.UpdatedDatetime = DateTime.UtcNow.AddHours(7);

                await _unitOfWork.FAQRepo.UpdateAsync(faq);

                response.Success = true;
                response.Message = "Faq updated successfully.";
                response.Data = _mapper.Map<ViewFaqDto>(faq);
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
