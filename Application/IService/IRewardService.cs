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
        public Task<ServiceResponse<IEnumerable<Reward>>> GetAllReward();
        public Task<ServiceResponse<AddReward>> AddReward(AddReward reward);
        public Task<ServiceResponse<Reward>> UpdateReward();
        public Task<ServiceResponse<int>> DeleteReward(int rewardId);
    }
}
