namespace Domain.Entities
{
    public class ProjectCategory
    {
        public int CategoryId { get; set; }
        public int ProjectId { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual Project Project { get; set; } = null!;
    }
}
