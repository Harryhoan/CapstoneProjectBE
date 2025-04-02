using Application.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IUnitOfWork
    {
        public IUserRepo UserRepo { get; }
        public IPledgeRepo PledgeRepo { get; }
        public ITokenRepo TokenRepo { get; }
        public IProjectRepo ProjectRepo { get; }
        public ICategoryRepo CategoryRepo { get; }
        public IRewardRepo RewardRepo { get; }
        public IFAQRepo FAQRepo { get; }
        public IPostRepo PostRepo { get; }
        public IReportRepo ReportRepo { get; }
        public ICommentRepo CommentRepo { get; }
        public IPostCommentRepo PostCommentRepo { get; }
        public IProjectCommentRepo ProjectCommentRepo { get; }
        public IPledgeDetailRepo PledgeDetailRepo { get; }
        public ICollaboratorRepo CollaboratorRepo { get; }
        public IFileRepo FileRepo { get; }
        public IProjectCategoryRepo ProjectCategoryRepo { get; }
        public IPlatformRepo PlatformRepo { get; }
        public IProjectPlatformRepo ProjectPlatformRepo { get; }

        public Task<int> SaveChangeAsync();
    }
}
