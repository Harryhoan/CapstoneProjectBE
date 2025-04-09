using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.CommentDTO
{
    public class CreatePostCommentDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Parent Comment ID must be a positive integer")]
        public int? ParentCommentId { get; set; }
        [Required(ErrorMessage = "Post ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Post ID must be a positive integer")]
        public int PostId { get; set; }
        [Required(ErrorMessage = "Content is required")]
        [StringLength(255, ErrorMessage = "Content can't be longer than 50 characters")]
        public string Content { get; set; } = string.Empty;
    }
}
