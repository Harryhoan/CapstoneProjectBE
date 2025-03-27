using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories
{
    public interface ITokenRepo : IGenericRepo<Token>
    {
        public Task<Token?> GetTokenWithUser(string tokenValue, string type);
        public Task<Token?> GetTokenByUserIdAsync(int userId);
        public Task<Token?> GetTokenByValueAsync(string tokenValue);
        public Task<Token?> FindByConditionAsync(int userId, string type);
    }
}
