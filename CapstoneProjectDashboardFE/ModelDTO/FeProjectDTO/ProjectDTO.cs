using Domain.Enums;
using Newtonsoft.Json;

namespace CapstoneProjectDashboardFE.ModelDTO.FeProjectDTO
{
    public class ProjectDTO
    {
        [JsonProperty("project-id")]
        public int ProjectId { get; set; }
        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; } = string.Empty;
        [JsonProperty("monitor")]
        public string Monitor { get; set; } = string.Empty;
        [JsonProperty("creator-id")]
        public int CreatorId { get; set; }
        [JsonProperty("creator")]
        public string Creator { get; set; } = string.Empty;
        [JsonProperty("title")]
        public string? Title { get; set; }
        [JsonProperty("status")]
        public ProjectEnum Status { get; set; }
        [JsonProperty("end-datetime")]
        public DateTime EndDatetime { get; set; }
    }
}
