using Application.ServiceResponse;
using PayPal.Api;

namespace Application.IService
{
    public interface IPaypalPaymentService
    {
        Task<ServiceResponse<string>> CreatePaymentAsync(int userId, int projectId, decimal amount, string returnUrl, string cancelUrl);
        Task<ServiceResponse<Payment>> ExecutePaymentAsync(string paymentId, string payerId);
        Task<ServiceResponse<string>> CreateRefundAsync(int userId, int pledgeId);
        Task<ServiceResponse<string>> TransferPledgeToCreatorAsync(int userId, int projectId);
        public Task<ServiceResponse<string>> RefundAllPledgesForProjectAsync(int projectId);
        public Task<ServiceResponse<string>> GetTransactionIdByInvoiceIdAsync(string invoiceId);
        public Task<ServiceResponse<string>> CreateInvoiceAsync(string itemName, decimal itemPrice, int quantity);
    }
}
