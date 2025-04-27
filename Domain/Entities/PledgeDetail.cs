using Domain.Enums;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class PledgeDetail
    {
        public int PledgeId { get; set; }
        public string PaymentId { get; set; } = string.Empty;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PledgeDetailEnum Status { get; set; }
        public decimal Amount { get; set; }
        public string InvoiceId { get; set; } = string.Empty;
        public string InvoiceUrl { get; set; } = string.Empty;
        public DateTime CreatedDatetime { get; set; } = DateTime.UtcNow.AddHours(7);
        public virtual Pledge Pledge { get; set; } = null!;
    }
}
