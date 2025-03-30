using Newtonsoft.Json;

namespace CapstoneProjectDashboardFE.ModelDTO.FeProjectDTO
{
    public class RewardDTO
    {
        [JsonProperty("reward-id")]
        public int RewardId { get; set; }
        [JsonProperty("project-id")]
        public int ProjectId { get; set; }
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
        [JsonProperty("details")]
        public string Details { get; set; } = string.Empty;
        [JsonProperty("created-datetime")]
        public DateTime CreatedDatetime { get; set; }
    }
}
