using Application.IRepositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class FileRepo : GenericRepo<Domain.Entities.File>, IFileRepo
    {
        private readonly ApiContext _dbContext;

        public FileRepo(ApiContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Domain.Entities.File>> GetFilesByUserId(int userId)
        {
            return await _dbContext.Files.Where(p => p.UserId == userId).ToListAsync();
        }

    }
}
