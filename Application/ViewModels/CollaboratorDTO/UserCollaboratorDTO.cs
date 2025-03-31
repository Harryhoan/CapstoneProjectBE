using Application.ViewModels.ProjectDTO;
using Application.ViewModels.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.CollaboratorDTO
{
    public class UserCollaboratorDTO
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public string Role { get; set; } = string.Empty;
        public PostUserDTO User { get; set; } = new PostUserDTO();
    }
}
