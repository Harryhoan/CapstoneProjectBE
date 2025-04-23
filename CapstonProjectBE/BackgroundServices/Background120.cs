
using Application;
using Application.IService;
using Application.Utils;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CapstonProjectBE.BackgroundServices
{
    public class Background120 : BackgroundService
    {
        private readonly ILogger<Background120> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Background120(ILogger<Background120> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        private async Task UpdateProjectAsync()
        {
            if (_serviceProvider != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApiContext>();

                if (dbContext != null)
                {
                    using var transaction = await dbContext.Database.BeginTransactionAsync();
                    try
                    {
                        var paypalPaymentService = scope.ServiceProvider.GetRequiredService<IPaypalPaymentService>();
                        var projects = await dbContext.Projects.Include(p => p.User).Include(p => p.Monitor).Where(p => p.User != null && !string.IsNullOrEmpty(p.User.Email) && p.Status != Domain.Enums.ProjectStatusEnum.DELETED).ToListAsync();
                        foreach (var project in projects)
                        {
                            //What about invisible projects? Can't the money from invisible projects be transferred as well?
                            if (project.TransactionStatus == Domain.Enums.TransactionStatusEnum.RECEIVING && DateTime.UtcNow.ToLocalTime() >= project.EndDatetime)
                            {
                                project.TransactionStatus = Domain.Enums.TransactionStatusEnum.PENDING;
                                dbContext.Update(project);
                                if (paypalPaymentService != null)
                                {
                                    if (project.TotalAmount > 0 && project.MinimumAmount > project.TotalAmount)
                                    {
                                        //var pledges = dbContext.Pledges.Where(p => p.ProjectId == project.ProjectId && p.Amount > 0).AsNoTracking().ToList();
                                        //foreach (var pledge in pledges)
                                        //{
                                        //    await paypalPaymentService.CreateRefundAsync(pledge.UserId, pledge.PledgeId);
                                        //}
                                        var emailSend = await EmailSender.SendHaltedProjectStatusEmailToCreator(project.User.Email, string.IsNullOrEmpty(project.Title) ? "[No Title]" : project.Title, false);
                                        if (!emailSend)
                                        {

                                        }
                                        if (project.Monitor != null && !string.IsNullOrEmpty(project.Monitor.Email))
                                        {
                                            emailSend = await EmailSender.SendHaltedProjectStatusEmailToMonitor(project.Monitor.Email, string.IsNullOrEmpty(project.Title) ? "[No Title]" : project.Title, project.ProjectId, false);
                                        }
                                    }
                                    else if (project.TotalAmount > 0)
                                    {
                                        await paypalPaymentService.TransferPledgeToCreatorAsync(project.CreatorId, project.ProjectId);
                                        var emailSend = await EmailSender.SendHaltedProjectStatusEmailToCreator(project.User.Email, string.IsNullOrEmpty(project.Title) ? "[No Title]" : project.Title, true);
                                        if (!emailSend)
                                        {

                                        }
                                        if (project.Monitor != null && !string.IsNullOrEmpty(project.Monitor.Email))
                                        {
                                            emailSend = await EmailSender.SendHaltedProjectStatusEmailToMonitor(project.Monitor.Email, string.IsNullOrEmpty(project.Title) ? "[No Title]" : project.Title, project.ProjectId, true);
                                        }
                                    }
                                }
                            }
                            var projectCategories = await dbContext.ProjectCategories.Include(pc => pc.Category).Where(pc => pc.Category != null && pc.Category.ParentCategoryId.HasValue).ToListAsync();
                            if (projectCategories.Any())
                            {
                                foreach (ProjectCategory projectCategory in projectCategories)
                                {
                                    if (projectCategory.Category.ParentCategoryId.HasValue)
                                    {
                                        var existingParentCategory = await dbContext.Categories.AsNoTracking().SingleOrDefaultAsync(c => c.CategoryId == projectCategory.Category.ParentCategoryId.Value);
                                        if (existingParentCategory == null)
                                        {
                                            projectCategory.Category.ParentCategoryId = null;
                                        }
                                        else if (existingParentCategory.ParentCategoryId.HasValue && existingParentCategory.ParentCategoryId.Value > 0 && projectCategories.FirstOrDefault(pc => pc.CategoryId == existingParentCategory.CategoryId) == null)
                                        {
                                            var parent = await dbContext.Categories.AsNoTracking().SingleOrDefaultAsync(c => c.CategoryId == existingParentCategory.ParentCategoryId.Value);
                                            if (parent != null)
                                            {
                                                var newProjectCategory = new ProjectCategory
                                                {
                                                    ProjectId = project.ProjectId,
                                                    CategoryId = (int)projectCategory.Category.ParentCategoryId
                                                };
                                                await dbContext.AddAsync(newProjectCategory);
                                            }
                                            parent = null;
                                            existingParentCategory = null;
                                        }
                                    }
                                }
                            }
                            await dbContext.SaveChangesAsync();
                        }
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background Service is starting.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    await UpdateProjectAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Background processing per 120 seconds failed");
                    Console.WriteLine($"Background processing per 120 seconds failed: {ex.Message}");
                }
                await Task.Delay(TimeSpan.FromSeconds(120), stoppingToken);
            }

        }
    }
}
