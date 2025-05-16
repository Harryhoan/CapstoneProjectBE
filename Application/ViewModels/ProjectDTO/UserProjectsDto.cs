using Domain.Enums;

namespace Application.ViewModels.ProjectDTO
{
    public class UserProjectsDto
    {
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Thumbnail { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime EndDatetime { get; set; }
        public TransactionStatusEnum TransactionStatus { get; set; }
        public ProjectStatusEnum Status { get; set; }
    }
}
