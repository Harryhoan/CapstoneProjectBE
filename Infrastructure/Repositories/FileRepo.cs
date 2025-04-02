using Application.IRepositories;
using Domain;
using Microsoft.EntityFrameworkCore;

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
