using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.UserDTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,15}$", ErrorMessage = "Password must be from 8-15 characters, at least one uppercase letter, one lowercase letter, one number and one special character!")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(50, ErrorMessage = "Title can't be longer than 50 characters")]
        public string Fullname { get; set; } = string.Empty;
    }
}
