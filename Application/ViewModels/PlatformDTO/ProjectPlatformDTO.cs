using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.PlatformDTO
{
    public class ProjectPlatformDTO
    {
        [Required(ErrorMessage = "Platform ID is required")]
        //[Range(1, int.MaxValue, ErrorMessage = "Platform ID must be a positive integer")]
        public int PlatformId { get; set; }
        [Required(ErrorMessage = "Project ID is required")]
        //[Range(1, int.MaxValue, ErrorMessage = "Project ID must be a positive integer")]
        public int ProjectId { get; set; }

    }
}
