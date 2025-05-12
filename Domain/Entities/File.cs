using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class File
    {
        [Key]
        public int FileId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime CreatedDatetime { get; set; } = DateTime.UtcNow.AddHours(7);
        public virtual User User { get; set; } = null!;
    }
}
