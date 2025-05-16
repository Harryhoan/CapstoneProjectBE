using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public int? ParentCommentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Status { get; set; } = "Created";
        public DateTime CreatedDatetime { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime UpdatedDatetime { get; set; } = DateTime.UtcNow.AddHours(7);
        public virtual User User { get; set; } = null!;
        public virtual Comment? ParentComment { get; set; }
        public virtual ProjectComment? ProjectComment { get; set; }
        public virtual PostComment? PostComment { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
