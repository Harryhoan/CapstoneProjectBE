using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectDashboardFE.ModelDTO.FeUserDTO
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
    public class LoginResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public int Hint { get; set; }
    }
}
