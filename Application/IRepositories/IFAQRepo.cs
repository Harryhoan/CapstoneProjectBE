using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories
{
    public interface IFAQRepo : IGenericRepo<FAQ>
    {
        Task<FAQ?> GetQuestionByQuestionAndProjectId(int projectId, string question);
        Task<List<FAQ>> GetAllQuestionsByProjectIdAsync(int projectId);
    }
}
