using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
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

        public async Task<ServiceResponse<ViewReward>> AddReward(AddReward reward)
        {
            var response = new ServiceResponse<ViewReward>();

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
                var project = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", reward.ProjectId);
                if (project == null || project.Status == Domain.Enums.ProjectStatusEnum.DELETED)
                {
                    response.Success = false;
                    response.Message = "The project cannot be found and may have already been deleted";
                    return response;
                }
                if (await _unitOfWork.RewardRepo.Any(r => r.ProjectId == reward.ProjectId && r.Amount == reward.Amount))
                {
                    response.Success = false;
                    response.Message = "There's already a reward with that amount for this project";
                    return response;
                }
                reward.Details = FormatUtils.FormatText(reward.Details);
                var newReward = _mapper.Map<Reward>(reward);
                newReward.CreatedDatetime = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
                await _unitOfWork.RewardRepo.AddAsync(newReward);

                response.Data = _mapper.Map<ViewReward>(newReward);
                response.Success = true;
                response.Message = "Reward created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create reward: {ex.Message}";
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
                response.Message = $"Failed to delete reward: {ex.Message}";
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
                var result = await _unitOfWork.RewardRepo.GetAllAsNoTrackingAsync();

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
                response.Message = $"Failed to get rewards: {ex.Message}";
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
                var result = await _unitOfWork.RewardRepo.GetByIdNoTrackingAsync("RewardId", rewardId);

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
                response.Message = $"Failed to get reward: {ex.Message}";
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return response;
        }

        public async Task<ServiceResponse<IEnumerable<ViewReward>>> GetRewardsByProjectId(int projectId)
        {
            var response = new ServiceResponse<IEnumerable<ViewReward>>();

            try
            {
                var rewards = await _unitOfWork.RewardRepo.GetAllAsNoTrackingAsync();

                var filteredRewards = rewards?.Where(r => r.ProjectId == projectId).ToList();

                if (filteredRewards == null || !filteredRewards.Any())
                {
                    response.Success = true;
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
                response.Message = $"Failed to get rewards: {ex.Message}";
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
                var existingProject = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", existingReward.ProjectId);
                if (existingProject == null)
                {
                    response.Success = false;
                    response.Message = "Project not found.";
                    return response;
                }
                if (existingProject.Status != Domain.Enums.ProjectStatusEnum.REJECTED && existingProject.Status != Domain.Enums.ProjectStatusEnum.CREATED)
                {
                    response.Success = false;
                    response.Message = "Rewards for " + existingProject.Status.ToString() + " projects are not changeable.";
                    return response;
                }
                if (await _unitOfWork.RewardRepo.Any(r => r.ProjectId == existingReward.ProjectId && r.RewardId != existingReward.RewardId && r.Amount == updateReward.Amount))
                {
                    response.Success = false;
                    response.Message = "There's already a reward with that amount for this project";
                    return response;
                }

                existingReward.Amount = updateReward.Amount;
                existingReward.Details = FormatUtils.FormatText(updateReward.Details);

                await _unitOfWork.RewardRepo.UpdateAsync(existingReward);

                var viewReward = _mapper.Map<ViewReward>(existingReward);

                response.Data = viewReward;
                response.Success = true;
                response.Message = "Reward updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to update reward: {ex.Message}";
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return response;
        }
    }
}
