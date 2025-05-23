using Application.ViewModels.CategoryDTO;
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
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ProjectStatusEnum Status { get; set; }
        //public TransactionStatusEnum TransactionStatus { get; set; }
        public decimal MinimumAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime StartDatetime { get; set; }
        public DateTime EndDatetime { get; set; }
        public DateTime CreatedDatetime { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public DateTime UpdatedDatetime { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public List<ViewCategory> Categories { get; set; } = new List<ViewCategory>();
        public List<PlatformDTO.PlatformDTO> Platforms { get; set; } = new List<PlatformDTO.PlatformDTO>();

    }
}
