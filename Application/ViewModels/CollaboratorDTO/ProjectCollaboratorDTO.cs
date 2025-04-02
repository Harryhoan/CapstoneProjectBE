using Application.ViewModels.ProjectDTO;
using Domain.Enums;

namespace Application.ViewModels.CollaboratorDTO
{
    public class ProjectCollaboratorDTO
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public CollaboratorEnum Role { get; set; }
        public ProjectDto Project { get; set; } = new ProjectDto();
    }
}
