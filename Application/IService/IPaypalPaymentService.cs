using Application.ServiceResponse;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IPaypalPaymentService
    {
        Task<ServiceResponse<string>> CreatePaymentAsync(int userId, int projectId, decimal amount, string returnUrl, string cancelUrl);
        Task<ServiceResponse<Payment>> ExecutePaymentAsync(string paymentId, string payerId);
        Task<ServiceResponse<string>> CreateRefundAsync(int userId, int pledgeId);
        Task<ServiceResponse<string>> TransferPledgeToCreatorAsync(int userId, int projectId);
    }
}
