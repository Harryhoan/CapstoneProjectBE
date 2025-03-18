using Application.IRepositories;
using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels.CategoryDTO;
using Application.ViewModels.GoalDTO;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class GoalService : IGoalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GoalService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<Goal>> AddGoal(int projectId, CreateGoal createGoal)
        {
            var response = new ServiceResponse<Goal>();

            try
            {
                var projectExists = await _unitOfWork.ProjectRepo.Find(p => p.ProjectId == projectId);
                if (!projectExists)
                {
                    response.Success = false;
                    response.Message = "Project not found.";
                    return response;
                }
                var newGoal = _mapper.Map<Goal>(createGoal);
                newGoal.ProjectId = projectId;
                await _unitOfWork.GoalRepo.AddAsync(newGoal);

                response.Data = newGoal;
                response.Success = true;
                response.Message = "Goal created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return response;
        }

        public Task<ServiceResponse<int>> DeleteGoal(int goalId)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<IEnumerable<Goal>>> GetAllGoal()
        {
            var response = new ServiceResponse<IEnumerable<Goal>>();

            try
            {
                var result = await _unitOfWork.GoalRepo.GetAllAsync();
                if (result != null && result.Any())
                {
                    response.Data = result;
                    response.Success = true;
                    response.Message = "Goals retrieved successfully.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "No goals found.";
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

        public Task<ServiceResponse<Goal>> UpdateGoal()
        {
            throw new NotImplementedException();
        }
    }
}
