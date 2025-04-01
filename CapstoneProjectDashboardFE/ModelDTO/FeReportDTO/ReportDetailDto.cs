using Newtonsoft.Json;

namespace CapstoneProjectDashboardFE.ModelDTO.FeReportDTO
{
    public class ReportDetailDto
    {
        [JsonProperty("report-id")]
        public int ReportId { get; set; }
        [JsonProperty("user-name")]
        public string Username { get; set; } = string.Empty;
        [JsonProperty("detail")]
        public string Detail { get; set; } = string.Empty;
        [JsonProperty("create-datetime")]
        public DateTime CreateDatetime { get; set; }
    }
}
