using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.ProjectDTO
{
    public class ProjectThumbnailDto
    {
        [Required(ErrorMessage = "Thumbnail is required")]
        [Url]
        public string Thumbnail {  get; set; } = string.Empty;
    }
}
