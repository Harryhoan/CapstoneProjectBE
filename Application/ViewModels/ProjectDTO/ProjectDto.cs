using Application.ViewModels.PlatformDTO;
using Domain.Enums;

namespace Application.ViewModels.ProjectDTO
{
    public class ProjectDto
    {
        public int ProjectId { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public string Monitor { get; set; } = string.Empty;
        public int CreatorId { get; set; }
        public string Creator { get; set; } = string.Empty;
        public int Backers { get; set; } = 0;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ProjectEnum Status { get; set; }
        public decimal MinimumAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime StartDatetime { get; set; }
        public DateTime EndDatetime { get; set; }
        //public List<ProjectCategoryDto> Categories { get; set; } = new List<ProjectCategoryDto>();
        //public List<ProjectPlatformDTO> Platforms { get; set; } = new List<ProjectPlatformDTO>();

    }
}
