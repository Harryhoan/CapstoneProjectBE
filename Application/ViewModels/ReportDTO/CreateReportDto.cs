using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.ReportDTO
{
    public class CreateReportDto
    {
        [Required(ErrorMessage = "Detail cannot be empty")]
        public string Detail { get; set; } = string.Empty;
    }
}
