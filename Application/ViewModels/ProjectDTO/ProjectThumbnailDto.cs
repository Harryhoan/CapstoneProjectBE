using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.ProjectDTO
{
    public class ProjectThumbnailDto
    {
        [Required(ErrorMessage = "Thumbnail is required")]
        [Url]
        public string Thumbnail { get; set; } = string.Empty;
    }
}
