using Application;
using Application.IRepositories;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddInfrastructuresService(this IServiceCollection services)
        {
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<ITokenRepo, TokenRepo>();
            services.AddScoped<IPledgeRepo, PledgeRepo>();
            services.AddScoped<IProjectRepo, ProjectRepo>();
            services.AddScoped<ICategoryRepo, CategoryRepo>();
            services.AddScoped<IRewardRepo, RewardRepo>();
            services.AddScoped<IReportRepo, ReportRepo>();
            services.AddScoped<IFAQRepo, FAQRepo>();
            services.AddScoped<IPledgeDetailRepo, PledgeDetailRepo>();
            services.AddScoped<IPostRepo, PostRepo>();
            services.AddScoped<ICommentRepo, CommentRepo>();
            services.AddScoped<IPostCommentRepo, PostCommentRepo>();
            services.AddScoped<IProjectCommentRepo, ProjectCommentRepo>();
            services.AddScoped<ICollaboratorRepo, CollaboratorRepo>();
            services.AddScoped<IPlatformRepo, PlatformRepo>();
            services.AddScoped<IFileRepo, FileRepo>();
            services.AddScoped<IProjectPlatformRepo, ProjectPlatformRepo>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProjectCategoryRepo, ProjectCategoryRepo>();
            services.AddScoped<IVerifyCodeRepo, VerifyCodeRepo>();
            return services;
        }
    }
}
