using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.CommentDTO
{
    public class CreateProjectCommentDTO
    {
        public int? ParentCommentId { get; set; }
        [Required(ErrorMessage = "Project ID is required")]
        public int ProjectId { get; set; }
        [Required(ErrorMessage = "Content is required")]
        [StringLength(255, ErrorMessage = "Content can't be longer than 50 characters")]
        public string Content { get; set; } = string.Empty;
    }
}
