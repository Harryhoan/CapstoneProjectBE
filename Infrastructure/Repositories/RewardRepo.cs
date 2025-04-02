using Application.IRepositories;
using Domain;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class RewardRepo : GenericRepo<Reward>, IRewardRepo
    {
        public RewardRepo(ApiContext context) : base(context)
        {
        }
    }
}
