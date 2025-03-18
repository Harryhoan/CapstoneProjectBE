using Application.IRepositories;
using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels.CategoryDTO;
using Application.ViewModels.RewardDTO;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var newReward = _mapper.Map<Reward>(reward);
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

        public Task<ServiceResponse<int>> DeleteReward(int rewardId)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<IEnumerable<Reward>>> GetAllReward()
        {
            var response = new ServiceResponse<IEnumerable<Reward>>();

            try
            {
                var result = await _unitOfWork.RewardRepo.GetAllAsync();
                if (result != null && result.Any())
                {
                    response.Data = result;
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

        public Task<ServiceResponse<Reward>> UpdateReward()
        {
            throw new NotImplementedException();
        }
    }
}
