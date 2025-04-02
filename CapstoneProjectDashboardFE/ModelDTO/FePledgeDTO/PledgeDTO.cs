using Newtonsoft.Json;

namespace CapstoneProjectDashboardFE.ModelDTO.FePledgeDTO
{
    public class PledgeDTO
    {
        [JsonProperty("pledge-id")]
        public int PledgeId { get; set; }
        [JsonProperty("user-id")]
        public int UserId { get; set; }
        [JsonProperty("project-id")]
        public int ProjectId { get; set; }
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}
