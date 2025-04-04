using Newtonsoft.Json;

namespace CapstoneProjectDashboardFE.ModelDTO.FePledgeDTO
{
    public class PledgeDetailDTO
    {
        [JsonProperty("payment-id")]
        public string PaymentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
