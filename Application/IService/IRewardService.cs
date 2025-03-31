using Application.ServiceResponse;
using Application.ViewModels.CategoryDTO;
using Application.ViewModels.RewardDTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IRewardService
    {
        public Task<ServiceResponse<IEnumerable<ViewReward>>> GetAllReward();
        public Task<ServiceResponse<IEnumerable<ViewReward>>> GetRewardByProjectId(int projectId);
        public Task<ServiceResponse<ViewReward>> GetRewardById(int rewardId);
        public Task<ServiceResponse<AddReward>> AddReward(AddReward reward);
        public Task<ServiceResponse<ViewReward>> UpdateReward(int rewardId, UpdateReward updateReward);
        public Task<ServiceResponse<int>> DeleteReward(int rewardId);
    }
}
