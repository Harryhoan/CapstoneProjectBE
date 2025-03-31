using Application.ViewModels.ProjectDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.CollaboratorDTO
{
    public class ProjectCollaboratorDTO
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public string Role { get; set; } = string.Empty;
        public ProjectDto Project { get; set; } = new ProjectDto();
    }
}
