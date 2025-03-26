using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories
{
    public interface IFileRepo : IGenericRepo<Domain.Entities.File>
    {
        public Task<List<Domain.Entities.File>> GetFilesByUserId(int userId);
    }
}
