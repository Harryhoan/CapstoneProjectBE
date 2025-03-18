using Application;
using Application.IRepositories;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            services.AddScoped<IGoalRepo, GoalRepo>();
            services.AddScoped<IPledgeDetailRepo, PledgeDetailRepo>();
            services.AddScoped<IPostRepo, PostRepo>();
            services.AddScoped<ICommentRepo, CommentRepo>();
            services.AddScoped<IPostCommentRepo, PostCommentRepo>();
            services.AddScoped<IProjectCommentRepo, ProjectCommentRepo>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
