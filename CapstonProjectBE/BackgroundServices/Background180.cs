using Application;
using Application.IService;
using Application.Utils;
using Domain;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PayPal.Api;
using System.ComponentModel.DataAnnotations;

namespace CapstonProjectBE.BackgroundServices
{
    public class Background180 : BackgroundService
    {
        private readonly ILogger<Background180> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public Background180(IConfiguration configuration, ILogger<Background180> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        private async Task Update()
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
                        var pledgeDetails = await dbContext.PledgeDetails.Where(pd => pd.Status == Domain.Enums.PledgeDetailEnum.REFUNDING || pd.Status == Domain.Enums.PledgeDetailEnum.TRANSFERRING).ToListAsync();
                        var apiContext = new PayPal.Api.APIContext(
                            new PayPal.Api.OAuthTokenCredential(
                                _configuration["PayPal:ClientId"],
                                _configuration["PayPal:ClientSecret"]
                            ).GetAccessToken());
                        if (pledgeDetails.Count > 0)
                        {
                            int i = 0;
                            while (i < pledgeDetails.Count)
                            {
                                var pledge = await dbContext.Pledges.SingleOrDefaultAsync(p => p.PledgeId == pledgeDetails[i].PledgeId);
                                if (pledge != null)
                                {
                                    var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.ProjectId == pledge.ProjectId);
                                    if (project != null)
                                    {
                                        if (string.IsNullOrWhiteSpace(pledgeDetails[i].PaymentId))
                                        {
                                            pledge.TotalAmount += pledgeDetails[i].Amount;
                                            dbContext.PledgeDetails.Remove(pledgeDetails[i]);
                                            dbContext.Pledges.Update(pledge);
                                            pledgeDetails.RemoveAt(i);
                                            continue;
                                        }


                                        var payoutDetails = Payout.Get(apiContext, pledgeDetails[i].PaymentId);
                                        if (payoutDetails == null)
                                        {
                                            pledge.TotalAmount += pledgeDetails[i].Amount * 100 / 95;
                                            dbContext.PledgeDetails.Remove(pledgeDetails[i]);
                                            dbContext.Pledges.Update(pledge);
                                            pledgeDetails.RemoveAt(i);
                                            continue;
                                        }
                                        else
                                        {
                                            var payoutItem = payoutDetails.items.FirstOrDefault(/*i => i.payout_item.sender_item_id.Equals(pledge.PledgeId.ToString())*/);
                                            if (payoutItem == null || !(payoutItem.transaction_status == PayoutTransactionStatus.SUCCESS || payoutItem.transaction_status == PayoutTransactionStatus.PENDING || payoutItem.transaction_status == PayoutTransactionStatus.UNCLAIMED || payoutItem.transaction_status == PayoutTransactionStatus.ONHOLD || payoutItem.transaction_status == PayoutTransactionStatus.NEW))
                                            {
                                                pledge.TotalAmount += pledgeDetails[i].Amount * 100 / 95;
                                                dbContext.PledgeDetails.Remove(pledgeDetails[i]);
                                                dbContext.Pledges.Update(pledge);
                                                pledgeDetails.RemoveAt(i);
                                                continue;
                                            }
                                            else if (payoutItem.transaction_status == PayoutTransactionStatus.SUCCESS && !string.IsNullOrWhiteSpace(payoutItem.transaction_id))
                                            {
                                                var pledgeDetail = pledgeDetails[i];
                                                var baseUrl = _configuration["PayPal:Mode"] == "live" ? "https://www.paypal.com" : "https://sandbox.paypal.com";
                                                if (pledgeDetails[i].Status == PledgeDetailEnum.REFUNDING)
                                                {
                                                    pledgeDetail.Status = PledgeDetailEnum.REFUNDED;
                                                    project.Status = ProjectStatusEnum.REFUNDED;
                                                    //project.TransactionStatus = TransactionStatusEnum.REFUNDED;
                                                }
                                                if (pledgeDetails[i].Status == PledgeDetailEnum.TRANSFERRING)
                                                {
                                                    pledgeDetail.Status = PledgeDetailEnum.TRANSFERRED;
                                                    project.Status = ProjectStatusEnum.TRANSFERRED;
                                                    //project.TransactionStatus = TransactionStatusEnum.TRANSFERRED;
                                                }
                                                pledgeDetail.InvoiceId = payoutItem.transaction_id;
                                                pledgeDetail.InvoiceUrl = $"{baseUrl}/unifiedtransactions/?filter=0&query={payoutItem.transaction_id}";
                                                dbContext.PledgeDetails.Update(pledgeDetail);
                                                dbContext.Projects.Update(project);
                                                var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == pledge.UserId);

                                                if (project != null && user != null && !string.IsNullOrWhiteSpace(user.Email) && new EmailAddressAttribute().IsValid(user.Email))
                                                {
                                                    if (pledgeDetail.Status == PledgeDetailEnum.REFUNDED)
                                                    {
                                                        await EmailSender.SendRefundInvoiceEmail(user.Email, string.IsNullOrEmpty(project.Title) ? "[No Title]" : project.Title, pledgeDetails[i].Amount, pledgeDetails[i].InvoiceUrl, project.ProjectId);
                                                    }
                                                    else if (pledgeDetail.Status == PledgeDetailEnum.TRANSFERRED)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(user.Email) && new EmailAddressAttribute().IsValid(user.Email))
                                                        {
                                                            var emailSend = await EmailSender.SendTransferInvoiceEmail(user.Fullname, user.Email, pledgeDetails[i].Amount, string.IsNullOrEmpty(project.Title) ? "[No Title]" : project.Title, pledgeDetails[i].InvoiceUrl, project.StartDatetime, project.EndDatetime, project.Status,/* project.TransactionStatus,*/ project.ProjectId);
                                                            if (!emailSend)
                                                            {
                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        i++;
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
                    await Update();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Background processing per 180 seconds failed");
                    Console.WriteLine($"Background processing per 180 seconds failed: {ex.Message}");
                }
                await Task.Delay(TimeSpan.FromSeconds(180), stoppingToken);
            }

        }
    }
}
