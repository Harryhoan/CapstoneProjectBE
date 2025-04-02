namespace Application.ViewModels.ReportDTO
{
    public class ReportDto
    {
        public int ReportId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public DateTime CreateDatetime { get; set; }
    }
}
