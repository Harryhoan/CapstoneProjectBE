using Newtonsoft.Json;

namespace CapstoneProjectDashboardFE.ModelDTO.FeReportDTO
{
    public class ReportDto
    {
        [JsonProperty("report-id")]
        public int ReportId { get; set; }
        [JsonProperty("user-id")]
        public int UserId { get; set; }
        [JsonProperty("detail")]
        public string Detail { get; set; } = string.Empty;
        [JsonProperty("create-datetime")]
        public DateTime CreateDatetime { get; set; }
    }
}
