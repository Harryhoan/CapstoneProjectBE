using Application.ViewModels.VnpayDTO;
using Microsoft.AspNetCore.Http;

namespace Application.IService
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentUrl(double money, string description, HttpContext httpContext, int projectId, int userId);
        PaymentResult ProcessPaymentResult(IQueryCollection query);
    }
}
