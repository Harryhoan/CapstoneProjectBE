using Domain.Enums;

namespace Application.ViewModels.ProjectDTO
{
    public class UpdateProjectDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal MinimumAmount { get; set; }
        public DateTime StartDatetime { get; set; }
        public DateTime EndDatetime { get; set; }
    }
}
