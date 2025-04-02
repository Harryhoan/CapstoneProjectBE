namespace Domain.Entities
{
    public class PostComment
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        // Relationships
        public virtual Post Post { get; set; } = null!;
        public virtual Comment Comment { get; set; } = null!;
    }
}
