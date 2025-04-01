using Application;
using Application.IRepositories;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApiContext _apiContext;
        private readonly IUserRepo _userRepo;
        private readonly ITokenRepo _tokenRepo;
        private readonly IProjectRepo _projectRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IRewardRepo _rewardRepo;
        private readonly IFAQRepo _faqRepo;
        private readonly IPostRepo _postRepo;
        private readonly ICommentRepo _commentRepo;
        private readonly IPostCommentRepo _postCommentRepo;
        private readonly IPledgeRepo _pledgeRepo;
        private readonly IReportRepo _reportRepo;
        private readonly IPledgeDetailRepo _pledgeDetailRepo;
        private readonly IProjectCommentRepo _projectCommentRepo;
        private readonly ICollaboratorRepo _collaboratorRepo;
        private readonly IFileRepo _fileRepo;
        private readonly IProjectCategoryRepo _projectCategoryRepo;

        public UnitOfWork(ApiContext apiContext, IUserRepo userRepo, ITokenRepo tokenRepo, IProjectRepo projectRepo, IPostRepo postRepo, ICommentRepo commentRepo, IPostCommentRepo postCommentRepo, IProjectCommentRepo projectCommentRepo, IPledgeRepo pledgeRepo, IPledgeDetailRepo pledgeDetailRepo, 
            ICategoryRepo categoryRepo, IRewardRepo rewardRepo, IReportRepo reportRepo, ICollaboratorRepo collaboratorRepo, IFAQRepo faqRepo, IFileRepo fileRepo, IProjectCategoryRepo projectCategoryRepo)
        {
            _apiContext = apiContext;
            _tokenRepo = tokenRepo;
            _userRepo = userRepo;
            _projectRepo = projectRepo;
            _categoryRepo = categoryRepo;
            _rewardRepo = rewardRepo;
            _postRepo = postRepo;
            _commentRepo = commentRepo;
            _postCommentRepo = postCommentRepo;
            _projectCommentRepo = projectCommentRepo;
            _pledgeRepo = pledgeRepo;
            _pledgeDetailRepo = pledgeDetailRepo;
            _collaboratorRepo = collaboratorRepo;
            _reportRepo = reportRepo;
            _faqRepo = faqRepo;
            _fileRepo = fileRepo;
            _projectCategoryRepo = projectCategoryRepo;
        }

        public IUserRepo UserRepo => _userRepo;

        public ITokenRepo TokenRepo => _tokenRepo;

        public IReportRepo ReportRepo => _reportRepo;

        public IPledgeRepo PledgeRepo => _pledgeRepo;

        public IProjectRepo ProjectRepo => _projectRepo;

        public ICategoryRepo CategoryRepo => _categoryRepo;

        public IRewardRepo RewardRepo => _rewardRepo;

        public IFAQRepo FAQRepo => _faqRepo;

        public IPostRepo PostRepo => _postRepo;

        public ICommentRepo CommentRepo => _commentRepo;

        public IPostCommentRepo PostCommentRepo => _postCommentRepo;

        public IProjectCommentRepo ProjectCommentRepo => _projectCommentRepo;

        public IPledgeDetailRepo PledgeDetailRepo => _pledgeDetailRepo;
        public ICollaboratorRepo CollaboratorRepo => _collaboratorRepo;
        public IFileRepo FileRepo => _fileRepo;
        public IProjectCategoryRepo ProjectCategoryRepo => _projectCategoryRepo;
        public async Task<int> SaveChangeAsync()
        {
            try
            {
                return await _apiContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log exception details here
                throw new ApplicationException("An error occurred while saving changes.", ex);
            }
        }
    }
}
