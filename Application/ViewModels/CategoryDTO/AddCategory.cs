using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.CategoryDTO
{
    public class AddCategory
    {
        [Range(1, int.MaxValue, ErrorMessage = "Parent Category ID must be a positive integer")]
        public int? ParentCategoryId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name can't be longer than 50 characters")]
        public string Name { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }
    }
}
