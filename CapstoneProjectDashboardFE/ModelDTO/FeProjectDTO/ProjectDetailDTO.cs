using Domain.Enums;
using Newtonsoft.Json;

namespace CapstoneProjectDashboardFE.ModelDTO.FeProjectDTO
{
    public class ProjectDetailDTO
    {
        [JsonProperty("project-id")]
        public int ProjectId { get; set; }
        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; } = string.Empty;
        [JsonProperty("monitor-id")]
        public string Monitor { get; set; } = string.Empty;
        [JsonProperty("creator-id")]
        public int CreatorId { get; set; }
        [JsonProperty("creator")]
        public string Creator { get; set; } = string.Empty;
        [JsonProperty("story")]
        public string Story { get; set; } = string.Empty;
        [JsonProperty("backers")]
        public int Backers { get; set; } = 0;
        [JsonProperty("title")]
        public string? Title { get; set; }
        [JsonProperty("description")]
        public string? Description { get; set; }
        [JsonProperty("status")]
        public ProjectEnum Status { get; set; }
        [JsonProperty("minimum-amount")]
        public decimal MinimumAmount { get; set; }
        [JsonProperty("total-amount")]
        public decimal TotalAmount { get; set; }
        [JsonProperty("start-datetime")]
        public DateTime StartDatetime { get; set; } = DateTime.UtcNow;
        [JsonProperty("end-datetime")]
        public DateTime EndDatetime { get; set; } = DateTime.UtcNow;
    }
}
