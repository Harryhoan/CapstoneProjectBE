namespace Application.ViewModels.UserDTO
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string? Avatar { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Bio { get; set; }
        public DateTime CreatedDatetime { get; set; }
    }
}
