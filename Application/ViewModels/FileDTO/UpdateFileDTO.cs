using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.FileDTO
{
    public class UpdateFileDTO
    {
        [Required(ErrorMessage = "File ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "File ID must be a positive integer")]
        public int FileId { get; set; }
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Deleted|Private|Exclusive|Public)$", ErrorMessage = "Status can only be \"Deleted\", \"Private\", \"Exclusive\" or \"Public\"")]
        public string Status { get; set; } = string.Empty;
        [Required(ErrorMessage = "Source is required")]
        public string Source { get; set; } = string.Empty;
    }
}
