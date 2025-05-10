using Domain.Enums;

namespace Application.ViewModels.PledgeDTO
{
    public class PledgeDetailDto
    {
        public decimal Amount { get; set; }
        public string PaymentId { get; set; } = string.Empty;
        public PledgeDetailEnum Status { get; set; }
        public string InvoiceId { get; set; } = string.Empty;
        public string InvoiceUrl { get; set; } = string.Empty;
        public DateTime CreatedDatetime { get; set; } = DateTime.UtcNow.AddHours(7);
    }
}
