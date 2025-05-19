using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Token
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ExpiresAt { get; set; } = DateTime.Now.AddMinutes(30);
        public string TokenValue { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        // Foreign key
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
