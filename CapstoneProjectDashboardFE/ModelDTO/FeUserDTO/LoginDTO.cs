using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectDashboardFE.ModelDTO.FeUserDTO
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
    public class LoginResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int Hint { get; set; }
    }
}
