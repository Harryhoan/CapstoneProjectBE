using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Report
    {
        [Key]
        public int ReportId { get; set; }
        public int UserId { get; set; }
        public string Detail { get; set; } = string.Empty;
        public DateTime CreateDatetime { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);

        public virtual User User { get; set; } = null!;
    }
}
