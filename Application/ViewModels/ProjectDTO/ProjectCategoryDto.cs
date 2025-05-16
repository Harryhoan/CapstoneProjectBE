using Domain.Enums;

namespace Application.ViewModels.ProjectDTO
{
    public class ProjectCategoryDto
    {
        public int CategoryId { get; set; }
        public int? ParentCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? CategoryDescription { get; set; }
        /// <summary>
        ///     
        /// </summary>
        public int ProjectId { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public string Monitor { get; set; } = string.Empty;
        public int CreatorId { get; set; }
        public string Creator { get; set; } = string.Empty;
        public int Backers { get; set; } = 0;
        public string Title { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;
        public ProjectStatusEnum Status { get; set; }
        public decimal MinimumAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime StartDatetime { get; set; }
        public DateTime EndDatetime { get; set; }
    }
}
