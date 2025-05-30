﻿
using Application;
using Application.IService;
using Application.Utils;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
                        var projects = await dbContext.Projects.Include(p => p.User).Include(p => p.Monitor).Where(p => p.User != null && !string.IsNullOrWhiteSpace(p.User.Email) && p.Status != Domain.Enums.ProjectStatusEnum.DELETED).ToListAsync();
                        foreach (var project in projects)
                        {
                            if (project.Status == Domain.Enums.ProjectStatusEnum.APPROVED && DateTime.UtcNow.AddHours(7) >= project.StartDatetime)
                            {
                                project.Status = Domain.Enums.ProjectStatusEnum.ONGOING;
                                dbContext.Update(project);
                            }
                            //What about invisible projects? Can't the money from invisible projects be transferred as well?
                            if (project.Status == Domain.Enums.ProjectStatusEnum.ONGOING && DateTime.UtcNow.AddHours(7) >= project.EndDatetime)
                            {
                                //project.TransactionStatus = Domain.Enums.TransactionStatusEnum.PENDING;
                                //dbContext.Update(project);
                                //if (paypalPaymentService != null)
                                //{
                                if (project.MinimumAmount > project.TotalAmount)
                                {
                                    //var pledges = dbContext.Pledges.Where(p => p.ProjectId == project.ProjectId && p.Amount > 0).AsNoTracking().ToList();
                                    //foreach (var pledge in pledges)
                                    //{
                                    //    await paypalPaymentService.CreateRefundAsync(pledge.UserId, pledge.PledgeId);
                                    //}
                                    project.Status = Domain.Enums.ProjectStatusEnum.INSUFFICIENT;
                                    dbContext.Update(project);
                                    var emailSend = await EmailSender.SendHaltedProjectStatusEmailToCreator(project.User.Email, string.IsNullOrEmpty(project.Title) ? "[No Title]" : project.Title, false);
                                    if (!emailSend)
                                    {

                                    }
                                    if (project.Monitor != null && !string.IsNullOrWhiteSpace(project.Monitor.Email) && new EmailAddressAttribute().IsValid(project.Monitor.Email))
                                    {
                                        emailSend = await EmailSender.SendHaltedProjectStatusEmailToMonitor(project.Monitor.Email, string.IsNullOrEmpty(project.Title) ? "[No Title]" : project.Title, project.ProjectId, false);
                                    }
                                }
                                else if (project.TotalAmount >= project.MinimumAmount)
                                {
                                    project.Status = Domain.Enums.ProjectStatusEnum.SUCCESSFUL;
                                    dbContext.Update(project);
                                    //await paypalPaymentService.TransferPledgeToCreatorAsync(project.CreatorId, project.ProjectId);
                                    var emailSend = await EmailSender.SendHaltedProjectStatusEmailToCreator(project.User.Email, string.IsNullOrEmpty(project.Title) ? "[No Title]" : project.Title, true);
                                    if (!emailSend)
                                    {

                                    }
                                    if (project.Monitor != null && !string.IsNullOrWhiteSpace(project.Monitor.Email) && !(new EmailAddressAttribute().IsValid(project.Monitor.Email)))
                                    {
                                        emailSend = await EmailSender.SendHaltedProjectStatusEmailToMonitor(project.Monitor.Email, string.IsNullOrEmpty(project.Title) ? "[No Title]" : project.Title, project.ProjectId, true);
                                    }
                                }
                                //}
                            }
                            if (project.Monitor == null || string.IsNullOrWhiteSpace(project.Monitor.Email) || !(new EmailAddressAttribute().IsValid(project.Monitor.Email)) || project.Monitor.IsDeleted)
                            {
                                var users = dbContext.Users.AsNoTracking().AsEnumerable();
                                if (users.Any(u => u.IsVerified && !u.IsDeleted && u.Role == Domain.Enums.UserEnum.STAFF && !string.IsNullOrWhiteSpace(u.Email) && new EmailAddressAttribute().IsValid(u.Email)))
                                {
                                    project.MonitorId = users.Where(u => u.IsVerified && !u.IsDeleted && u.Role == Domain.Enums.UserEnum.STAFF && !string.IsNullOrWhiteSpace(u.Email) && new EmailAddressAttribute().IsValid(u.Email)).OrderBy(u => u.MonitoredProjects.Count).First().UserId;
                                    dbContext.Update(project);
                                }
                                else if (users.Any(u => u.IsVerified && !u.IsDeleted && u.Role == Domain.Enums.UserEnum.ADMIN && !string.IsNullOrWhiteSpace(u.Email) && new EmailAddressAttribute().IsValid(u.Email)))
                                {
                                    project.MonitorId = users.Where(u => u.IsVerified && !u.IsDeleted && u.Role == Domain.Enums.UserEnum.ADMIN && !string.IsNullOrWhiteSpace(u.Email) && new EmailAddressAttribute().IsValid(u.Email)).OrderBy(u => u.MonitoredProjects.Count).First().UserId;
                                    dbContext.Update(project);
                                }
                                else
                                {
                                    project.MonitorId = project.CreatorId;
                                    dbContext.Update(project);
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
