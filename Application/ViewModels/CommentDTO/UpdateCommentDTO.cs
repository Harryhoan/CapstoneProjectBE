using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.CommentDTO
{
    public class UpdateCommentDTO
    {
        [Required(ErrorMessage = "Comment ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Comment ID must be a positive integer")]
        public int CommentId { get; set; }
        [Required(ErrorMessage = "Content is required")]
        [StringLength(500, ErrorMessage = "Content can't be longer than 50 characters")]
        public string Content { get; set; } = string.Empty;
    }
}
