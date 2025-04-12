using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using PayPal;
using PayPal.Api;

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
                if (project.Status == ProjectEnum.DELETED)
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

                foreach (var item in pledgeDetails)
                {
                    if (item.Status == PledgeDetailEnum.TRANSFERRED)
                    {
                        response.Success = false;
                        response.Message = "The money has already been transferred.";
                        return response;
                    }
                    //if (item.Status == PledgeDetailEnum.REFUNDED)
                    //{
                    //    response.Success = false;
                    //    response.Message = "This pledge has already been transferred.";
                    //    return response;
                    //}
                }

                var finalTotalPledgeForCreator = project.TotalAmount - (project.TotalAmount * 5 / 100);
                var apiContext = new APIContext(new OAuthTokenCredential(
                    _configuration["PayPal:ClientId"],
                    _configuration["PayPal:ClientSecret"]
                ).GetAccessToken());

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
                        value = finalTotalPledgeForCreator.ToString(),
                        currency = "USD"
                    },
                    receiver = creator.Email,
                    note = "Transfer of pledged funds",
                    sender_item_id = projectId.ToString()
                }
            }
                };

                var createdPayout = payout.Create(apiContext, false); // Execute PayPal payout request

                //var transferPledge = new Pledge
                //{
                //    UserId = creator.UserId,
                //    ProjectId = projectId,
                //    Amount = finalTotalPledgeForCreator,
                //};

                //await _unitOfWork.PledgeRepo.AddAsync(transferPledge);

                //foreach (var pledgeDetail in pledgeDetails)
                //{
                //    pledgeDetail.Status = PledgeDetailEnum.TRANSFERRED;
                //    pledgeDetail.PaymentId = createdPayout.batch_header.payout_batch_id;
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
                    response.Message = "User deleted.";
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
                if (project.Status == ProjectEnum.DELETED)
                {
                    response.Success = false;
                    response.Message = "This project has been deleted.";
                    return response;
                }

                var apiContext = new APIContext(new OAuthTokenCredential(
                    _configuration["PayPal:ClientId"],
                    _configuration["PayPal:ClientSecret"]
                ).GetAccessToken());

                string totalAmountInUSD = pledge.Amount.ToString("F2");

                project.TotalAmount -= pledge.Amount;

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
                            receiver = user.PaymentAccount,
                            note = "Refund for your pledge",
                            sender_item_id = pledge.PledgeId.ToString()
                        }
                    }
                };

                var createdPayout = payout.Create(apiContext, false);

                //pledge.Amount = 0;
                //PledgeDetail pledgeDetail = new PledgeDetail();
                //pledgeDetail.PledgeId = pledge.PledgeId;
                //pledgeDetail.Status = PledgeDetailEnum.REFUNDED;
                await _unitOfWork.ProjectRepo.UpdateAsync(project);
                await _unitOfWork.PledgeRepo.UpdateAsync(pledge);

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

                if (project.Status == ProjectEnum.DELETED || project.Status == ProjectEnum.INVISIBLE)
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

                var apiContext = new APIContext(new OAuthTokenCredential(
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
                        if (user != null && !user.IsDeleted)
                        {
                            string totalAmountInUSD = pledge.Amount.ToString("F2");

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

                project.TotalAmount = 0;
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
                string totalAmount = amount.ToString("F2");

                var project = await _unitOfWork.ProjectRepo.GetByIdNoTrackingAsync("ProjectId", projectId);
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
                    response.Message = "Your account needs to be verified before using this method.";
                    return response;
                }
                if (project == null)
                {
                    response.Success = false;
                    response.Message = "Project Id not found.";
                    return response;
                }
                if (project.Status == ProjectEnum.DELETED || project.Status == ProjectEnum.INVISIBLE)
                {
                    response.Success = false;
                    response.Message = "This request is invalid.";
                    return response;
                }
                if (project.Status == ProjectEnum.HALTED)
                {
                    response.Success = false;
                    response.Message = "This project has been halted.";
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
                var apiContext = new APIContext(new OAuthTokenCredential(
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
                var apiContext = new APIContext(new OAuthTokenCredential(
                    _configuration["PayPal:ClientId"],
                    _configuration["PayPal:ClientSecret"]
                ).GetAccessToken());

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

                if (existingPledge == null)
                {
                    Domain.Entities.Pledge newPledge = new Domain.Entities.Pledge
                    {
                        UserId = userId,
                        Amount = amount,
                        ProjectId = projectId
                    };

                    await _unitOfWork.PledgeRepo.AddAsync(newPledge);

                    Domain.Entities.PledgeDetail pledgeDetail = new Domain.Entities.PledgeDetail
                    {
                        PledgeId = newPledge.PledgeId,
                        PaymentId = paymentId,
                        Status = PledgeDetailEnum.PLEDGED
                    };

                    await _unitOfWork.PledgeDetailRepo.AddAsync(pledgeDetail);
                }
                else
                {
                    existingPledge.Amount += amount;
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
                        Status = PledgeDetailEnum.PLEDGED
                    };

                    await _unitOfWork.PledgeDetailRepo.AddAsync(pledgeDetail);
                }

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

    }
}
