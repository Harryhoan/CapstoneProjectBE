using Application.ViewModels.UserDTO;

namespace Application.ViewModels.PostDTO
{
    public class PostDTO
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public DateTime CreatedDatetime { get; set; } = default;

        public PostUserDTO User { get; set; } = new PostUserDTO();
    }
}
