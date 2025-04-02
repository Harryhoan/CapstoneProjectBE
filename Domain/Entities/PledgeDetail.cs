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

        public virtual Pledge Pledge { get; set; } = null!;
    }
}
