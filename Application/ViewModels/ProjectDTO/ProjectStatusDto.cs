namespace Application.ViewModels.ProjectDTO
{
    public class ProjectStatusDTO
    {
        public int ProjectId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
