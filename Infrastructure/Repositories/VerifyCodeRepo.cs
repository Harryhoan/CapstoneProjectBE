using Application.IRepositories;
using Domain;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class VerifyCodeRepo : GenericRepo<VerifyCode>, IVerifyCodeRepo
    {
        public VerifyCodeRepo(ApiContext context) : base(context)
        {
        }
    }
}
