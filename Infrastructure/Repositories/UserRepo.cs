using Application.IRepositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Domain;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<User?> GetByEmailAsync(string sEmail) => await _dbSet.FirstOrDefaultAsync(u => u.Email == sEmail);
        public async Task<User> GetUserByEmailAddressAndPasswordHash(string email, string passwordHash)
        {
            var user = await _dbContext.Users.Include(u => u.Tokens)
                .FirstOrDefaultAsync(record => record.Email == email && record.Password == passwordHash);
            return user;
        }
        public async Task<IEnumerable<User>> GetAllUser()
        {
            return await _dbContext.Users.Where(u => u.Role == "Customer" || u.Role == "Staff").ToListAsync();
        }
        public int GetCount()
        {
            return _dbContext.Users.Count();
        }
    }
}
