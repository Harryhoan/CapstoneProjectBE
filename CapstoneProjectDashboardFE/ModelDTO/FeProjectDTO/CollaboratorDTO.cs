using Application.ViewModels.UserDTO;
using Domain.Enums;

namespace CapstoneProjectDashboardFE.ModelDTO.FeProjectDTO
{
    public class CollaboratorDTO
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public CollaboratorEnum Role { get; set; }
        public PostUserDTO User { get; set; } = new PostUserDTO();
    }
}
