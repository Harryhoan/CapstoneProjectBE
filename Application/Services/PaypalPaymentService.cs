using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Domain.Entities;
using Domain.Enums;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Pdfa;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;
using PayPal;
using PayPal.Api;
using System.ComponentModel.DataAnnotations;
using Paragraph = iText.Layout.Element.Paragraph;

namespace Application.Services
{
    public class PaypalPaymentService : IPaypalPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PaypalPaymentService(IConfiguration configuration, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ServiceResponse<string>> TransferPledgeToCreatorAsync(int userId, int projectId)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var project = await _unitOfWork.ProjectRepo.GetByIdAsync(projectId);

                if (project == null)
                {
                    response.Success = false;
                    response.Message = "Project not found.";
                    return response;
                }

                var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }
                if (user.IsDeleted)
                {
                    response.Success = false;
                    response.Message = "This request is invalid.";
                    return response;
                }
                if (!(user.Role == UserEnum.STAFF && user.UserId == project.MonitorId) && user.Role != UserEnum.ADMIN)
                {
                    response.Success = false;
                    response.Message = "You are not allowed to do this method.";
                    return response;
                }
                if (project.EndDatetime > DateTime.UtcNow)
                {
                    response.Success = false;
                    response.Message = "This project has not ended yet.";
                    return response;
                }
                if (project.Status == ProjectStatusEnum.DELETED)
                {
                    response.Success = false;
                    response.Message = "This request is invalid.";
                    return response;
                }
                if (project.TotalAmount < project.MinimumAmount)
                {
                    response.Success = false;
                    response.Message = "This project has not reached the minimum amount.";
                    return response;
                }
                if (project.TransactionStatus == TransactionStatusEnum.REFUNDED)
                {
                    response.Success = false;
                    response.Message = "The total amount of this project has been refunded.";
                    return response;
                }
                if (project.TransactionStatus == TransactionStatusEnum.TRANSFERED)
                {
                    response.Success = false;
                    response.Message = "The total amount of this project has been transferred to the creator.";
                    return response;
                }
                var creator = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", project.CreatorId);
                if (creator == null)
                {
                    response.Success = false;
                    response.Message = "Creator not found.";
                    return response;
                }
                if (creator.IsDeleted)
                {
                    response.Success = false;
                    response.Message = "Creator deleted.";
                    return response;
                }
                if (!creator.IsVerified)
                {
                    response.Success = false;
                    response.Message = "Creator unverified.";
                    return response;
                }
                if (string.IsNullOrEmpty(creator.PaymentAccount) || !new EmailAddressAttribute().IsValid(creator.PaymentAccount))
                {
                    response.Success = false;
                    response.Message = "Payment Account invalid.";
                    return response;
                }
                var pledges = await _unitOfWork.PledgeRepo.GetManyPledgeByUserIdAndProjectIdAsync(userId, projectId);
                if (pledges == null || !pledges.Any())
                {
                    response.Success = false;
                    response.Message = "No pledges found for this project.";
                    return response;
                }
                var pledgeDetails = new List<PledgeDetail>();

                foreach (var pledge in pledges)
                {
                    var details = await _unitOfWork.PledgeDetailRepo.GetPledgeDetailByPledgeId(pledge.PledgeId);
                    if (details != null)
                    {
                        pledgeDetails.AddRange(details);
                    }
                }

                //foreach (var item in pledgeDetails)
                //{
                //    if (item.Status == PledgeDetailEnum.TRANSFERRED)
                //    {
                //        response.Success = false;
                //        response.Message = "The money has already been transferred.";
                //        return response;
                //    }
                //}

                var finalTotalPledgeForCreator = project.TotalAmount - (project.TotalAmount * 5 / 100);
                if (finalTotalPledgeForCreator <= 0)
                {
                    response.Success = false;
                    response.Message = "There is no money to transfer.";
                    return response;
                }
                var apiContext = new PayPal.Api.APIContext(new PayPal.Api.OAuthTokenCredential(
                    _configuration["PayPal:ClientId"],
                    _configuration["PayPal:ClientSecret"]
                ).GetAccessToken());

                string invoiceNumber = $"INV-{projectId}-{creator.UserId}-{Guid.NewGuid()}";

                var payout = new Payout
                {
                    sender_batch_header = new PayoutSenderBatchHeader
                    {
                        sender_batch_id = Guid.NewGuid().ToString(),
                        email_subject = "You have received a pledge transfer"
                    },
                    items = new List<PayoutItem>
            {
                new PayoutItem
                {
                    recipient_type = PayoutRecipientType.EMAIL,
                    amount = new Currency
                    {
                        value = finalTotalPledgeForCreator.ToString("F2"),
                        currency = "USD"
                    },
                    receiver = creator.PaymentAccount,
                    note = "Transfer of pledged funds",
                    sender_item_id = invoiceNumber
                }
            }
                };

                var createdPayout = payout.Create(apiContext, false); // Execute PayPal payout request

                // Fetch the payout details to retrieve the transaction ID
                var payoutDetails = Payout.Get(apiContext, createdPayout.batch_header.payout_batch_id);
                var transactionId = payoutDetails.items.FirstOrDefault()?.transaction_id;
                var invoiceUrl = payoutDetails.links.FirstOrDefault(link => link.rel == "self")?.href;

                if (string.IsNullOrEmpty(transactionId))
                {
                    response.Success = false;
                    response.Message = "Transaction ID not found for the payout item.";
                    return response;
                }
                var transferPledge = new Pledge
                {
                    UserId = creator.UserId,
                    ProjectId = projectId,
                    TotalAmount = finalTotalPledgeForCreator,
                };

                await _unitOfWork.PledgeRepo.AddAsync(transferPledge);

                // Create a new PledgeDetail with the updated properties
                var transferPledgeDetail = new PledgeDetail
                {
                    PledgeId = transferPledge.PledgeId,
                    PaymentId = createdPayout.batch_header.payout_batch_id,
                    InvoiceId = invoiceNumber,
                    Amount = finalTotalPledgeForCreator,
                    InvoiceUrl = invoiceUrl ?? string.Empty,
                    Status = PledgeDetailEnum.TRANSFERRED
                };

                await _unitOfWork.PledgeDetailRepo.AddAsync(transferPledgeDetail);

                //foreach (var pledgeDetail in pledgeDetails)
                //{
                //    pledgeDetail.Status = PledgeDetailEnum.TRANSFERRED;
                //    pledgeDetail.PaymentId = createdPayout.batch_header.payout_batch_id;
                //    pledgeDetail.TransactionId = transactionId;
                //}

                await _unitOfWork.SaveChangeAsync();

                response.Success = true;
                response.Message = "Pledge transfer completed successfully.";
            }
            catch (PayPalException paypalEx)
            {
                response.Success = false;
                response.Message = $"PayPal error: {paypalEx.Message}";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create payment: {ex.Message}";
            }

            return response;
        }


        public async Task<ServiceResponse<string>> CreateRefundAsync(int userId, int pledgeId)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var pledge = await _unitOfWork.PledgeRepo.GetByIdAsync(pledgeId);

                if (pledge == null)
                {
                    response.Success = false;
                    response.Message = "Pledge not found.";
                    return response;
                }

                var user = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                if (user.IsDeleted)
                {
                    response.Success = false;
                    response.Message = "User deleted.";
                    return response;
                }
                if (!user.IsVerified)
                {
                    response.Success = false;
                    response.Message = "User unverified.";
                    return response;
                }

                var project = await _unitOfWork.ProjectRepo.GetByIdAsync(pledge.ProjectId);
                if (project == null)
                {
                    response.Success = false;
                    response.Message = "Project not found.";
                    return response;
                }
                if (project.StartDatetime < project.EndDatetime)
                {
                    response.Success = false;
                    response.Message = "This project has not ended yet.";
                    return response;
                }
                if (project.TotalAmount >= project.MinimumAmount)
                {
                    response.Success = false;
                    response.Message = "This project has reached the minimum amount.";
                    return response;
                }
                if (project.Status == ProjectStatusEnum.DELETED)
                {
                    response.Success = false;
                    response.Message = "This project has been deleted.";
                    return response;
                }
                if (project.TransactionStatus == TransactionStatusEnum.REFUNDED)
                {
                    response.Success = false;
                    response.Message = "The total amount of this project has been refunded.";
                    return response;
                }
                if (project.TransactionStatus == TransactionStatusEnum.TRANSFERED)
                {
                    response.Success = false;
                    response.Message = "The total amount of this project has been transferred to the creator.";
                    return response;
                }
                var refundUser = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", pledge.UserId);
                if (refundUser == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }
                if (refundUser.IsDeleted)
                {
                    response.Success = false;
                    response.Message = "User deleted.";
                    return response;
                }
                if (!refundUser.IsVerified)
                {
                    response.Success = false;
                    response.Message = "User unverified.";
                    return response;
                }
                if (string.IsNullOrEmpty(refundUser.PaymentAccount) || !new EmailAddressAttribute().IsValid(refundUser.PaymentAccount))
                {
                    response.Success = false;
                    response.Message = "Payment Account invalid.";
                    return response;
                }

                var finalTotalAmount = pledge.TotalAmount - (pledge.TotalAmount * 5 / 100);
                if (finalTotalAmount <= 0)
                {
                    response.Success = false;
                    response.Message = "There is no money to refund.";
                    return response;
                }
                string totalAmountInUSD = finalTotalAmount.ToString("F2");

                project.TotalAmount -= pledge.TotalAmount;
                var apiContext = new PayPal.Api.APIContext(new PayPal.Api.OAuthTokenCredential(
                    _configuration["PayPal:ClientId"],
                    _configuration["PayPal:ClientSecret"]
                ).GetAccessToken());

                var payout = new Payout
                {
                    sender_batch_header = new PayoutSenderBatchHeader
                    {
                        sender_batch_id = Guid.NewGuid().ToString(),
                        email_subject = "You have a refund from your pledge"
                    },
                    items = new List<PayoutItem>
                    {
                        new PayoutItem
                        {
                            recipient_type = PayoutRecipientType.EMAIL,
                            amount = new Currency
                            {
                                value = totalAmountInUSD,
                                currency = "USD"
                            },
                            receiver = refundUser.PaymentAccount,
                            note = "Refund for your pledge",
                            sender_item_id = pledge.PledgeId.ToString()
                        }
                    }
                };
                await _unitOfWork.ProjectRepo.UpdateAsync(project);

                var createdPayout = payout.Create(apiContext, false);
                var payoutDetails = Payout.Get(apiContext, createdPayout.batch_header.payout_batch_id);
                var transactionId = payoutDetails.items.FirstOrDefault()?.transaction_id;
                var invoiceUrl = payoutDetails.links.FirstOrDefault(link => link.rel == "self")?.href;

                if (string.IsNullOrEmpty(transactionId))
                {
                    response.Success = false;
                    response.Message = "Transaction ID not found for the payout item.";
                    return response;
                }

                pledge.TotalAmount = 0;
                PledgeDetail pledgeDetail = new()
                {
                    PledgeId = pledge.PledgeId,
                    Status = PledgeDetailEnum.REFUNDED,
                    InvoiceId = transactionId,
                    Amount = finalTotalAmount,
                    InvoiceUrl = invoiceUrl ?? string.Empty
                };
                await _unitOfWork.PledgeRepo.UpdateAsync(pledge);
                await _unitOfWork.PledgeDetailRepo.AddAsync(pledgeDetail);
                response.Success = true;
                response.Message = "Payout created successfully.";
            }
            catch (PayPalException payPalEx)
            {
                response.Success = false;
                response.Message = $"PayPal error: {payPalEx.Message}";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create payout: {ex.Message}";
            }
            return response;
        }
        public async Task<ServiceResponse<string>> RefundAllPledgesForProjectAsync(int projectId)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var project = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
                if (project == null)
                {
                    response.Success = false;
                    response.Message = "Project not found.";
                    return response;
                }

                if (project.Status == ProjectStatusEnum.DELETED)
                {
                    response.Success = false;
                    response.Message = "This request is invalid.";
                    return response;
                }
                if (project.EndDatetime > DateTime.UtcNow)
                {
                    response.Success = false;
                    response.Message = "This project has not ended yet.";
                    return response;
                }
                var pledges = await _unitOfWork.PledgeRepo.GetPledgeByProjectIdAsync(projectId);
                if (pledges == null || !pledges.Any())
                {
                    response.Success = false;
                    response.Message = "No pledges found for this project.";
                    return response;
                }

                var apiContext = new PayPal.Api.APIContext(new PayPal.Api.OAuthTokenCredential(
                    _configuration["PayPal:ClientId"],
                    _configuration["PayPal:ClientSecret"]
                ).GetAccessToken());

                var payoutItems = new List<PayoutItem>();

                foreach (var pledge in pledges)
                {
                    var pledgeDetails = await _unitOfWork.PledgeDetailRepo.GetPledgeDetailByPledgeId(pledge.PledgeId);
                    if (!pledgeDetails.Any(pd => pd.Status == PledgeDetailEnum.REFUNDED))
                    {
                        var user = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", pledge.UserId);
                        if (user != null && !user.IsDeleted && user.IsVerified)
                        {
                            var finalTotalAmount = pledge.TotalAmount - (pledge.TotalAmount * 5 / 100);
                            if (finalTotalAmount > 0)
                            {
                                string totalAmountInUSD = finalTotalAmount.ToString("F2");

                                payoutItems.Add(new PayoutItem
                                {
                                    recipient_type = PayoutRecipientType.EMAIL,
                                    amount = new Currency
                                    {
                                        value = totalAmountInUSD,
                                        currency = "USD"
                                    },
                                    receiver = user.PaymentAccount,
                                    note = "Refund for your pledge",
                                    sender_item_id = pledge.PledgeId.ToString()
                                });
                                await _unitOfWork.SaveChangeAsync();
                            }
                        }
                    }
                }

                var payout = new Payout
                {
                    sender_batch_header = new PayoutSenderBatchHeader
                    {
                        sender_batch_id = Guid.NewGuid().ToString(),
                        email_subject = "You have a refund from your pledge"
                    },
                    items = payoutItems
                };

                var createdPayout = payout.Create(apiContext, false);
                foreach (var pledge in pledges)
                {
                    var pledgeDetails = await _unitOfWork.PledgeDetailRepo.GetPledgeDetailByPledgeId(pledge.PledgeId);
                    if (!pledgeDetails.Any(pd => pd.Status == PledgeDetailEnum.REFUNDED))
                    {
                        var user = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", pledge.UserId);
                        if (user != null && !user.IsDeleted && user.IsVerified)
                        {
                            var payoutDetails = Payout.Get(apiContext, createdPayout.batch_header.payout_batch_id);
                            var transactionId = payoutDetails.items.FirstOrDefault(i => i.payout_item.sender_item_id.Equals(pledge.PledgeId.ToString()))?.transaction_id;
                            //var invoiceUrl = payoutDetails.links.FirstOrDefault(link => link.rel == "self")?.href;

                            if (!string.IsNullOrEmpty(transactionId))
                            {
                                var finalTotalAmount = pledge.TotalAmount - (pledge.TotalAmount * 5 / 100);

                                pledge.TotalAmount = 0;
                                PledgeDetail pledgeDetail = new()
                                {
                                    PaymentId = createdPayout.batch_header.payout_batch_id,
                                    PledgeId = pledge.PledgeId,
                                    Status = PledgeDetailEnum.REFUNDED,
                                    InvoiceId = transactionId,
                                    Amount = finalTotalAmount,
                                    InvoiceUrl = $"https://sandbox.paypal.com/unifiedtransactions/?filter=0&query={transactionId}"
                                };
                                await _unitOfWork.PledgeRepo.UpdateAsync(pledge);
                                await _unitOfWork.PledgeDetailRepo.AddAsync(pledgeDetail);
                            }
                        }
                    }
                }


                project.TotalAmount = 0;
                project.TransactionStatus = TransactionStatusEnum.REFUNDED;
                await _unitOfWork.ProjectRepo.UpdateAsync(project);

                response.Success = true;
                response.Message = "All pledges refunded successfully.";
            }
            catch (PayPalException payPalEx)
            {
                response.Success = false;
                response.Message = $"PayPal error: {payPalEx.Message}";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to refund pledges: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<string>> CreatePaymentAsync(int userId, int projectId, decimal amount, string returnUrl, string cancelUrl)
        {
            var response = new ServiceResponse<string>();

            try
            {
                if (amount > 100000)
                {
                    response.Success = false;
                    response.Message = "The amount can not exceed 100000$ in one back.";
                    return response;
                }
                if (!(amount > 0))
                {
                    response.Success = false;
                    response.Message = "The amount of the payment must be above 0.";
                    return response;
                }
                var user = await _unitOfWork.UserRepo.GetByIdNoTrackingAsync("UserId", userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }
                if (user.Role != UserEnum.CUSTOMER)
                {
                    response.Success = false;
                    response.Message = "You are forbidden from pledging.";
                    return response;
                }
                if (user.IsVerified == false)
                {
                    response.Success = false;
                    response.Message = "Your account is not verified. Missing Phone Number or Payment Account.";
                    return response;
                }
                var project = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
                if (project == null)
                {
                    response.Success = false;
                    response.Message = "Project Id not found.";
                    return response;
                }
                if (project.Status == ProjectStatusEnum.DELETED)
                {
                    response.Success = false;
                    response.Message = "This request is invalid.";
                    return response;
                }

                if (project.TransactionStatus != TransactionStatusEnum.RECEIVING)
                {
                    response.Success = false;
                    response.Message = "This project is currently not accepting any pledge.";
                    return response;
                }
                if (project.EndDatetime <= DateTime.UtcNow)
                {
                    response.Success = false;
                    response.Message = "This project has ended.";
                    return response;
                }
                if (project.CreatorId == userId)
                {
                    response.Success = false;
                    response.Message = "You are not allowed to pledge to your own Game Project.";
                    return response;
                }
                string totalAmount = amount.ToString("F2");
                var apiContext = new PayPal.Api.APIContext(new PayPal.Api.OAuthTokenCredential(
                    _configuration["PayPal:ClientId"],
                    _configuration["PayPal:ClientSecret"]
                ).GetAccessToken());

                // Generate a unique invoice number
                string uniqueInvoiceNumber = $"P{projectId}-U{userId}-{Guid.NewGuid()}";

                var payment = new Payment
                {
                    intent = "sale",
                    payer = new Payer { payment_method = "paypal" },
                    transactions = new List<Transaction>
                    {
                        new Transaction
                        {
                            description = $"Pledge to project {projectId} by user {userId} - Payment",
                            invoice_number = uniqueInvoiceNumber,
                            amount = new Amount
                            {
                                currency = "USD",
                                total = totalAmount
                            },
                            note_to_payee = projectId.ToString(),
                            custom = userId.ToString()
                        }
                    },
                    redirect_urls = new RedirectUrls
                    {
                        cancel_url = cancelUrl,
                        return_url = returnUrl
                    }
                };

                var createdPayment = payment.Create(apiContext);
                if (createdPayment != null && createdPayment.links != null)
                {
                    var approvalLink = createdPayment.links.FirstOrDefault(link => link.rel == "approval_url")?.href;

                    if (approvalLink != null)
                    {
                        response.Success = true;
                        response.Message = "Payment created successfully.";
                        response.Data = approvalLink;
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Approval URL not found.";
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "Failed to create payment.";
                }

            }
            catch (PayPalException payPalEx)
            {
                response.Success = false;
                response.Error = $"PayPal error: {payPalEx.Message}";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = $"Failed to create payment: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<Payment>> ExecutePaymentAsync(string paymentId, string payerId)
        {
            var response = new ServiceResponse<Payment>();
            try
            {
                var apiContext = new PayPal.Api.APIContext(new PayPal.Api.OAuthTokenCredential(
                    _configuration["PayPal:ClientId"],
                    _configuration["PayPal:ClientSecret"]
                ).GetAccessToken())
                {
                    Config = new Dictionary<string, string>
            {
                { "mode", "sandbox" }
            }
                };

                var payment = Payment.Get(apiContext, paymentId);

                if (payment == null || string.IsNullOrEmpty(payment.state) || payment.transactions.Count == 0)
                {
                    response.Success = false;
                    response.Message = "Payment not found or invalid.";
                    return response;
                }

                var transaction = payment.transactions.FirstOrDefault();
                if (transaction == null || string.IsNullOrEmpty(transaction.custom) || string.IsNullOrEmpty(transaction.note_to_payee))
                {
                    response.Success = false;
                    response.Message = "Invalid transaction details.";
                    return response;
                }

                if (!int.TryParse(transaction.custom, out int userId) || userId <= 0 || !int.TryParse(transaction.note_to_payee, out int projectId))
                {
                    response.Success = false;
                    response.Message = "Invalid user or project ID.";
                    return response;
                }

                if (payment.state != "created")
                {
                    response.Success = false;
                    response.Message = "Payment is not approved yet.";
                    return response;
                }

                var project = await _unitOfWork.ProjectRepo.GetByIdAsync(projectId);

                if (project == null)
                {
                    response.Success = false;
                    response.Message = "Project Id not found.";
                    return response;
                }

                project.TotalAmount += Convert.ToDecimal(transaction.amount.total);

                decimal amount = decimal.TryParse(transaction.amount.total, out decimal parsedAmount) ? parsedAmount : 0m;

                var existingPledge = await _unitOfWork.PledgeRepo.GetPledgeByUserIdAndProjectIdAsync(userId, projectId);

                // Prepare to execute the payment
                await _unitOfWork.ProjectRepo.UpdateAsync(project);
                var paymentExecution = new PaymentExecution() { payer_id = payerId };
                var executedPayment = payment.Execute(apiContext, paymentExecution);

                if (executedPayment == null || string.IsNullOrEmpty(executedPayment.state) || executedPayment.state == "failed")
                {
                    response.Success = false;
                    response.Message = "Payment executed unsuccessfully.";
                    return response;
                }

                // Retrieve the invoice number from the transaction
                var invoiceNumber = transaction.invoice_number;

                if (string.IsNullOrEmpty(invoiceNumber))
                {
                    response.Success = false;
                    response.Message = "Invoice number not found.";
                    return response;
                }

                if (existingPledge == null)
                {
                    Domain.Entities.Pledge newPledge = new Domain.Entities.Pledge
                    {
                        UserId = userId,
                        TotalAmount = amount,
                        ProjectId = projectId
                    };

                    await _unitOfWork.PledgeRepo.AddAsync(newPledge);

                    Domain.Entities.PledgeDetail pledgeDetail = new Domain.Entities.PledgeDetail
                    {
                        PledgeId = newPledge.PledgeId,
                        PaymentId = paymentId,
                        Amount = amount,
                        InvoiceId = invoiceNumber, // Store the invoice number in the transactionId field
                        InvoiceUrl = $"https://www.sandbox.paypal.com/unifiedtransactions/?filter=0&query={invoiceNumber}",
                        Status = PledgeDetailEnum.PLEDGED
                    };

                    await _unitOfWork.PledgeDetailRepo.AddAsync(pledgeDetail);
                }
                else
                {
                    existingPledge.TotalAmount += amount;
                    await _unitOfWork.PledgeRepo.UpdateAsync(existingPledge);

                    var getPledge = await _unitOfWork.PledgeRepo.GetPledgeByUserIdAndProjectIdAsync(userId, projectId);
                    if (getPledge == null)
                    {
                        response.Success = false;
                        response.Message = "Pledge not found.";
                        return response;
                    }

                    Domain.Entities.PledgeDetail pledgeDetail = new Domain.Entities.PledgeDetail
                    {
                        PledgeId = getPledge.PledgeId,
                        PaymentId = paymentId,
                        Amount = amount,
                        InvoiceId = invoiceNumber, // Store the invoice number in the transactionId field
                        Status = PledgeDetailEnum.PLEDGED
                    };

                    await _unitOfWork.PledgeDetailRepo.AddAsync(pledgeDetail);
                }
                var userEmail = (await _unitOfWork.UserRepo.GetByIdAsync(userId))?.Email;
                //if (!string.IsNullOrEmpty(userEmail))
                //{
                //    // Generate the invoice
                //    var invoicePath = GenerateInvoice(userEmail, project.Title ?? "Null", amount, invoiceNumber);

                //    // Send the invoice via email
                //    await EmailSender.SendBillingEmail(userEmail, "Your Invoice", amount, "Please find your invoice attached.", invoicePath);
                //}

                response.Success = true;
                response.Message = "Payment successful.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to execute payment: {ex.Message}";
            }

            return response;
        }
        private string GenerateInvoice(string userEmail, string projectTitle, decimal amount, string invoiceNumber)
        {
            var invoicePath = Path.Combine(Path.GetTempPath(), $"{invoiceNumber}.pdf");

            // Define font and color resources
            string fontFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "FreeSans.ttf");
            string colorProfilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "sRGB_CS_profile.icm");

            using (var writer = new PdfWriter(invoicePath))
            {
                using (var pdf = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B,
                    new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1",
                    new FileStream(colorProfilePath, FileMode.Open, FileAccess.Read))))
                {
                    PdfFont font = PdfFontFactory.CreateFont(fontFile, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
                    Document document = new Document(pdf);
                    document.SetFont(font);

                    // Header
                    Paragraph header = new Paragraph("INVOICE")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(20)
                        .SetBold();
                    document.Add(header);

                    Paragraph subheader = new Paragraph("Generated using iText7 in .NET")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(10);
                    document.Add(subheader);

                    document.Add(new LineSeparator(new SolidLine()));

                    // Seller Details
                    Paragraph sellerHeader = new Paragraph("Sold by:")
                        .SetBold()
                        .SetTextAlignment(TextAlignment.LEFT);
                    Paragraph sellerDetail = new Paragraph("GameMkt")
                        .SetTextAlignment(TextAlignment.LEFT);
                    Paragraph sellerAddress = new Paragraph($"Project: {projectTitle}")
                        .SetTextAlignment(TextAlignment.LEFT);
                    Paragraph sellerContact = new Paragraph("+1 123-456-7890")
                        .SetTextAlignment(TextAlignment.LEFT);

                    document.Add(sellerHeader);
                    document.Add(sellerDetail);
                    document.Add(sellerAddress);
                    document.Add(sellerContact);

                    // Customer Details
                    Paragraph customerHeader = new Paragraph("Customer details:")
                        .SetBold()
                        .SetTextAlignment(TextAlignment.RIGHT);
                    Paragraph customerDetail = new Paragraph(userEmail)
                        .SetTextAlignment(TextAlignment.RIGHT);
                    Paragraph customerAddress = new Paragraph("Customer Address (if available)")
                        .SetTextAlignment(TextAlignment.RIGHT);

                    document.Add(customerHeader);
                    document.Add(customerDetail);
                    document.Add(customerAddress);

                    // Invoice Details
                    Paragraph invoiceDetails = new Paragraph($"Invoice No: {invoiceNumber}\nDate: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}")
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetBold();
                    document.Add(invoiceDetails);

                    // Table for Order Details
                    Table table = new Table(3, true);
                    table.SetFontSize(9);

                    // Table Headers
                    table.AddCell(new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph("Description")));
                    table.AddCell(new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph("Amount")));
                    table.AddCell(new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph("Total")));

                    // Table Data
                    table.AddCell(new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph(projectTitle)));
                    table.AddCell(new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph($"{amount:C}")));
                    table.AddCell(new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph($"{amount:C}")));

                    // Grand Total
                    table.AddCell(new Cell(1, 2).SetTextAlignment(TextAlignment.RIGHT).Add(new Paragraph("Grand Total:")));
                    table.AddCell(new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph($"{amount:C}")));

                    document.Add(table);
                    table.Flush();
                    table.Complete();

                    document.Close();
                }
            }

            return invoicePath;
        }

        public ServiceResponse<string> GetTransactionIdByInvoiceIdAsync(string invoiceId)
        {
            var response = new ServiceResponse<string>();

            try
            {
                var apiContext = new PayPal.Api.APIContext(new PayPal.Api.OAuthTokenCredential(
                    _configuration["PayPal:ClientId"],
                    _configuration["PayPal:ClientSecret"]
                ).GetAccessToken())
                {
                    Config = new Dictionary<string, string>
            {
                { "mode", "sandbox" } // Change to "live" for production
            }
                };

                // Search for payments using the invoice number
                var paymentHistory = Payment.List(apiContext, count: 10); // Retrieve the last 10 payments

                var payment = paymentHistory.payments
                    .FirstOrDefault(p => p.transactions.Any(t => t.invoice_number == invoiceId));

                if (payment != null)
                {
                    var transactionId = payment.transactions.FirstOrDefault()?.related_resources
                        ?.FirstOrDefault()?.sale?.id;

                    if (!string.IsNullOrEmpty(transactionId))
                    {
                        response.Success = true;
                        response.Data = transactionId;
                        response.Message = "Transaction ID retrieved successfully.";
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Transaction ID not found.";
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "No payments found for the given invoice ID.";
                }
            }
            catch (PayPalException ex)
            {
                response.Success = false;
                response.Message = $"PayPal error: {ex.Message}";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }
        public ServiceResponse<string> CreateInvoiceAsync(string itemName, decimal itemPrice, int quantity)
        {
            var response = new ServiceResponse<string>();

            try
            {
                // Set up API context
                var apiContext = new PayPal.Api.APIContext(new PayPal.Api.OAuthTokenCredential(
                    _configuration["PayPal:ClientId"],
                    _configuration["PayPal:ClientSecret"]
                ).GetAccessToken())
                {
                    Config = new Dictionary<string, string>
            {
                { "mode", "sandbox" } // Change to "live" for production
            }
                };

                // Create invoice object
                var invoice = new Invoice
                {
                    merchant_info = new MerchantInfo
                    {
                        email = "sb-41eyn38365498@business.example.com"
                    },
                    billing_info = new List<BillingInfo>
            {
                new BillingInfo
                {
                    email = "harryhoang203@gmail.com"
                }
            },
                    items = new List<InvoiceItem>
            {
                new InvoiceItem
                {
                    name = itemName,
                    quantity = quantity,
                    unit_price = new Currency
                    {
                        currency = "USD",
                        value = itemPrice.ToString("F2")
                    }
                }
            },
                    note = "Thank you for your business!",
                    payment_term = new PaymentTerm
                    {
                        term_type = "NET_30" // Payment due in 30 days
                    }
                };

                // Create the invoice
                var createdInvoice = invoice.Create(apiContext);

                if (createdInvoice != null && !string.IsNullOrEmpty(createdInvoice.id))
                {
                    // Optionally send the invoice
                    createdInvoice.Send(apiContext);

                    response.Success = true;
                    response.Data = createdInvoice.id;
                    response.Message = "Invoice created and sent successfully.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Failed to create the invoice.";
                }
            }
            catch (PayPalException ex)
            {
                response.Success = false;
                response.Message = $"PayPal error: {ex.Message}";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }
    }
}
