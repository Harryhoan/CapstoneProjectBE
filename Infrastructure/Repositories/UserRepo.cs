using Application.IRepositories;
using Domain;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepo : GenericRepo<User>, IUserRepo
    {
        private readonly ApiContext _dbContext;

        public UserRepo(ApiContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CheckEmailAddressExisted(string sEmail)
        {
            return await _dbContext.Users.AnyAsync(e => e.Email == sEmail);
        }

        public async Task<User?> GetByEmailAsync(string email) => await _dbSet.FirstOrDefaultAsync(u => u.Email.Trim().ToLower() == email.Trim().ToLower() && !u.IsDeleted);
        public async Task<User> GetUserByEmailAddressAndPasswordHash(string email, string passwordHash)
        {
            var user = await _dbContext.Users.Include(u => u.Tokens)
                .FirstOrDefaultAsync(record => record.Email == email && record.Password == passwordHash);
            return user;
        }
        public async Task<IEnumerable<User>> GetAllUser()
        {
            return await _dbContext.Users.Where(u => u.Role == UserEnum.CUSTOMER || u.Role == UserEnum.STAFF).ToListAsync();
        }
        public int GetCount()
        {
            return _dbContext.Users.Count();
        }
    }
}
