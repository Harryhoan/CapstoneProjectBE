using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.PlatformDTO
{
    public class CreatePlatformDTO
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name can't be longer than 50 characters")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

    }
}
