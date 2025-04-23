using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels.RewardDTO;
using AutoMapper;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.Services
{
    public class RewardService : IRewardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public RewardService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<AddReward>> AddReward(AddReward reward)
        {
            var response = new ServiceResponse<AddReward>();

            try
            {
                var validationContext = new ValidationContext(reward);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(reward, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                var newReward = _mapper.Map<Reward>(reward);
                newReward.CreatedDatetime = DateTime.UtcNow.ToLocalTime();
                await _unitOfWork.RewardRepo.AddAsync(newReward);

                response.Data = reward;
                response.Success = true;
                response.Message = "Reward created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return response;
        }

        public async Task<ServiceResponse<int>> DeleteReward(int rewardId)
        {
            var response = new ServiceResponse<int>();

            try
            {
                var existingReward = await _unitOfWork.RewardRepo.GetByIdAsync(rewardId);

                if (existingReward == null)
                {
                    response.Success = false;
                    response.Message = "Reward not found.";
                    return response;
                }

                await _unitOfWork.RewardRepo.RemoveAsync(existingReward);

                response.Data = rewardId;
                response.Success = true;
                response.Message = "Reward deleted successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<ViewReward>>> GetAllReward()
        {
            var response = new ServiceResponse<IEnumerable<ViewReward>>();

            try
            {
                var result = await _unitOfWork.RewardRepo.GetAllAsync();

                var responseData = new List<ViewReward>();
                foreach (var reward in result)
                {
                    var viewReward = new ViewReward
                    {
                        RewardId = reward.RewardId,
                        ProjectId = reward.ProjectId,
                        Amount = reward.Amount,
                        CreatedDatetime = reward.CreatedDatetime,
                        Details = reward.Details,
                    };
                    responseData.Add(viewReward);
                }

                if (result != null && result.Any())
                {
                    response.Data = responseData;
                    response.Success = true;
                    response.Message = "Rewards retrieved successfully.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "No reward found.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return response;
        }

        public async Task<ServiceResponse<ViewReward>> GetRewardById(int rewardId)
        {
            var response = new ServiceResponse<ViewReward>();

            try
            {
                var result = await _unitOfWork.RewardRepo.GetByIdAsync(rewardId);

                if (result == null)
                {
                    response.Success = false;
                    response.Message = "No reward found.";
                    return response;
                }

                var viewReward = new ViewReward
                {
                    RewardId = result.RewardId,
                    ProjectId = result.ProjectId,
                    Amount = result.Amount,
                    CreatedDatetime = result.CreatedDatetime,
                    Details = result.Details,
                };

                response.Data = viewReward;
                response.Success = true;
                response.Message = "Reward retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return response;
        }

        public async Task<ServiceResponse<IEnumerable<ViewReward>>> GetRewardByProjectId(int projectId)
        {
            var response = new ServiceResponse<IEnumerable<ViewReward>>();

            try
            {
                var rewards = await _unitOfWork.RewardRepo.GetAllAsync();

                var filteredRewards = rewards?.Where(r => r.ProjectId == projectId).ToList();

                if (filteredRewards == null || !filteredRewards.Any())
                {
                    response.Success = false;
                    response.Message = "No rewards found for the given project.";
                    return response;
                }

                response.Data = filteredRewards.Select(r => new ViewReward
                {
                    RewardId = r.RewardId,
                    ProjectId = r.ProjectId,
                    Amount = r.Amount,
                    CreatedDatetime = r.CreatedDatetime,
                    Details = r.Details
                }).ToList();

                response.Success = true;
                response.Message = "Rewards retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return response;
        }

        public async Task<ServiceResponse<ViewReward>> UpdateReward(int rewardId, UpdateReward updateReward)
        {
            var response = new ServiceResponse<ViewReward>();

            try
            {
                var validationContext = new ValidationContext(updateReward);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(updateReward, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                var existingReward = await _unitOfWork.RewardRepo.GetByIdAsync(rewardId);

                if (existingReward == null)
                {
                    response.Success = false;
                    response.Message = "Reward not found.";
                    return response;
                }

                existingReward.Amount = updateReward.Amount;
                existingReward.Details = updateReward.Details;

                await _unitOfWork.RewardRepo.UpdateAsync(existingReward);

                var viewReward = new ViewReward
                {
                    RewardId = existingReward.RewardId,
                    Amount = updateReward.Amount,
                    Details = updateReward.Details,
                    ProjectId = existingReward.ProjectId,
                };

                response.Data = viewReward;
                response.Success = true;
                response.Message = "Reward updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return response;
        }
    }
}
