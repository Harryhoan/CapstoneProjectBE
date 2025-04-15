using Domain.Enums;

namespace Application.ViewModels.ProjectDTO
{
    public class UserProjectsDto
    {
        public int ProjectId { get; set; }
        public string? Title { get; set; }
        public string? Thumbnail { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime EndDatetime { get; set; }
        public ProjectStatusEnum Status { get; set; }
    }
}
