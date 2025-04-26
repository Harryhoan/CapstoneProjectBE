using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.ProjectDTO
{
    public class UpdateProjectDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        [Range(0.01, 1000000, ErrorMessage = "Minimum amount cannot be smaller or equal to 0")]
        public decimal? MinimumAmount { get; set; }
        public DateTime? StartDatetime { get; set; }
        public DateTime? EndDatetime { get; set; }
    }
}
