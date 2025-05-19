namespace Domain.Entities
{
    public class FAQ
    {
        public int ProjectId { get; set; }
        public string Question { get; set; } = null!;
        public string Answer { get; set; } = null!;
        public DateTime CreatedDatetime { get; set; } = DateTime.Now;
        public DateTime UpdatedDatetime { get; set; } = DateTime.Now;
        public virtual Project Project { get; set; } = null!;
    }
}
