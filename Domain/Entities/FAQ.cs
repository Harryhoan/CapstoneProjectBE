namespace Domain.Entities
{
    public class FAQ
    {
        public int ProjectId { get; set; }
        public string Question { get; set; } = null!;
        public string Answer { get; set; } = null!;
        public DateTime CreatedDatetime { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime UpdatedDatetime { get; set; } = DateTime.UtcNow.AddHours(7);
        public virtual Project Project { get; set; } = null!;
    }
}
