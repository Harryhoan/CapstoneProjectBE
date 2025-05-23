using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.ProjectDTO
{
    public class CreateProjectDto
    {
        [Required(ErrorMessage = "Title cannot be empty")]
        [RegularExpression(@"^[^\s]+(\s+[^\s]+)*$", ErrorMessage = "Title cannot consist of only spaces.")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Description cannot be empty")]
        [RegularExpression(@"^[^\s]+(\s+[^\s]+)*$", ErrorMessage = "Description cannot consist of only spaces.")]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Minimum Amount cannot be empty")]
        [Range(0.01, 1000000, ErrorMessage = "Minimum amount cannot be smaller or equal to 0")]
        public decimal MinimumAmount { get; set; }

        [Required(ErrorMessage = "Start Date cannot be empty")]
        public DateTime StartDatetime { get; set; }

        [Required(ErrorMessage = "End Date cannot be empty")]
        public DateTime EndDatetime { get; set; }
    }
}
