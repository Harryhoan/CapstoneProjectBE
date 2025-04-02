namespace Domain.Entities
{
    public class FAQ
    {
        public int ProjectId { get; set; }
        public string Question { get; set; } = null!;
        public string Answer { get; set; } = null!;
        public DateTime CreatedDatetime { get; set; }
        public DateTime UpdatedDatetime { get; set; }
        public virtual Project Project { get; set; } = null!;
    }
}
