using Domain.Entities;

namespace Application.IRepositories
{
    public interface IUserRepo : IGenericRepo<User>
    {
        Task<bool> CheckEmailAddressExisted(string sEmail);
        Task<User?> GetByEmailAsync(string email);
        Task<User> GetUserByEmailAddressAndPasswordHash(string email, string passwordHash);
        Task<IEnumerable<User>> GetAllUser();
        int GetCount();
    }
}
