using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories
{
    public interface IPlatformRepo : IGenericRepo<Platform>
    {
        public Task<List<Platform>> GetPlatformsByNameOrDescriptionAsNoTracking(string query);
    }
}
