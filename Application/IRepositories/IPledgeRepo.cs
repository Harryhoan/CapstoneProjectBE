using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories
{
    public interface IPledgeRepo : IGenericRepo<Pledge>
    {
        Task<Pledge> GetPledgeByIdAsync(int id);
        Task<Pledge> GetPledgeByUserIdAndProjectIdAsync(int userId, int projectId);
        Task<List<Pledge>> GetManyPledgeByUserIdAndProjectIdAsync(int userId, int projectId);
        Task<List<Pledge>> GetPledgeByUserIdAsync(int userId);
    }
}
