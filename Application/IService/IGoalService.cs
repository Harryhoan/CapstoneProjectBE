using Application.ServiceResponse;
using Application.ViewModels.CategoryDTO;
using Application.ViewModels.GoalDTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IGoalService
    {
        public Task<ServiceResponse<IEnumerable<Goal>>> GetAllGoal();
        public Task<ServiceResponse<Goal>> AddGoal(int projectId, CreateGoal createGoal);
        public Task<ServiceResponse<Goal>> UpdateGoal();
        public Task<ServiceResponse<int>> DeleteGoal(int goalId);
    }
}
