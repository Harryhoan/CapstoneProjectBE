﻿using Application.ServiceResponse;
using Application.ViewModels.RewardDTO;

namespace Application.IService
{
    public interface IRewardService
    {
        public Task<ServiceResponse<IEnumerable<ViewReward>>> GetAllReward();
        public Task<ServiceResponse<IEnumerable<ViewReward>>> GetRewardsByProjectId(int projectId);
        public Task<ServiceResponse<ViewReward>> GetRewardById(int rewardId);
        public Task<ServiceResponse<ViewReward>> AddReward(AddReward reward);
        public Task<ServiceResponse<ViewReward>> UpdateReward(int rewardId, UpdateReward updateReward);
        public Task<ServiceResponse<int>> DeleteReward(int rewardId);
    }
}
