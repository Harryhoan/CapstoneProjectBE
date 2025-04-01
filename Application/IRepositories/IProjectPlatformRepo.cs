using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories
{
    public interface IProjectPlatformRepo : IGenericRepo<ProjectPlatform>
    {
        public Task<List<ProjectPlatform>> GetProjectPlatformsByPlatformId(int platformId);
        public Task<ProjectPlatform?> GetProjectPlatformByProjectIdAndPlatformId(int projectId, int platformId);
    }
}
