using Domain.Entities;

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
