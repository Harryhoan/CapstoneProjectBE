using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.ProjectDTO
{
    public class AddCategoryToProject
    {
        [Required(ErrorMessage = "Category ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be a positive integer")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Project ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Project ID must be a positive integer")]
        public int ProjectId { get; set; }

    }
}
