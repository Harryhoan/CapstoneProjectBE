using Application.IService;
using Application.Services;
using Application.Utils.Vnpay;
using Infrastructure;

namespace CapstonProjectBE
{
    public static class DependencyInject
    {
        public static IServiceCollection AddWebAPIService(this IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(option =>
            {
                option.JsonSerializerOptions.PropertyNamingPolicy = new KebabCaseNamingPolicy();
            });
            services.AddScoped<IVnpay, Vnpay>();
            services.AddScoped<IAuthenService, AuthenService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IRewardService, RewardService>();
            services.AddScoped<IFAQService, FAQService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPledgeService, PledgeService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IPaypalPaymentService, PaypalPaymentService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<ICollaboratorService, CollaboratorService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IPlatformService, PlatformService>();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHealthChecks();
            return services;
        }
    }
}
