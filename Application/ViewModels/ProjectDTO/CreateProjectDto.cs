using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.ProjectDTO
{
    public class CreateProjectDto
    {
        [Required(ErrorMessage = "Title cannot be empty")]
        public string? Title { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "Minimum Amount cannot be empty")]
        [Range(10, 1000000, ErrorMessage = "Minimum amount cannot be smaller or equal to 0")]
        public decimal MinimumAmount { get; set; }

        [Required(ErrorMessage = "Image URL cannot be empty")]
        public DateTime StartDatetime { get; set; }

        [Required(ErrorMessage = "End Date cannot be empty")]
        public DateTime EndDatetime { get; set; }
    }
}
