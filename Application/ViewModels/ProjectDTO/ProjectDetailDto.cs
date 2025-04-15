using Application.ViewModels.CategoryDTO;
using Application.ViewModels.PlatformDTO;
using Domain.Enums;

namespace Application.ViewModels.ProjectDTO
{
    public class ProjectDetailDto
    {
        public int ProjectId { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public string Monitor { get; set; } = string.Empty;
        public int CreatorId { get; set; }
        public string Creator { get; set; } = string.Empty;
        public string Story { get; set; } = string.Empty;
        public int Backers { get; set; } = 0;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ProjectStatusEnum Status { get; set; }
        public decimal MinimumAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime StartDatetime { get; set; }
        public DateTime EndDatetime { get; set; }
        public List<ViewCategory> Categories { get; set; } = new List<ViewCategory>();
        public List<PlatformDTO.PlatformDTO> Platforms { get; set; } = new List<PlatformDTO.PlatformDTO>();

    }
}
