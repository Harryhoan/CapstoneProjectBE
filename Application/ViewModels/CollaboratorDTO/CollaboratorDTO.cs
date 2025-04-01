using Application.ViewModels.ProjectDTO;
using Application.ViewModels.UserDTO;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.CollaboratorDTO
{
    public class CollaboratorDTO
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public CollaboratorEnum Role { get; set; }
        public ProjectDto Project {get; set;} = new ProjectDto();
        public PostUserDTO User { get; set; } = new PostUserDTO();
    }
}
