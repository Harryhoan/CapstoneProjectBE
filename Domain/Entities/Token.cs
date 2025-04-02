namespace Domain.Entities
{
    public class Token
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string TokenValue { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        // Foreign key
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
