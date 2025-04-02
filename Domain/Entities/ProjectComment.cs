namespace Domain.Entities
{
    public class ProjectComment
    {
        public int CommentId { get; set; }
        public int ProjectId { get; set; }

        public virtual Project Project { get; set; } = null!;
        public virtual Comment Comment { get; set; } = null!;
    }
}
