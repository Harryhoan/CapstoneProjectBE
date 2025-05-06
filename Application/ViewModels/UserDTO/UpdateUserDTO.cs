using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.UserDTO
{
    public class UpdateUserDTO
    {
        public string? Fullname { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }
        [EmailAddress(ErrorMessage = "Invalid payment account")]
        public string? PaymentAccount { get; set; }
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,15}$", ErrorMessage = "Password must be from 8-15 characters, at least one uppercase letter, one lowercase letter, one number and one special character!")]
        public string? Password { get; set; }
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits long and contain only numbers")]
        public string? Phone { get; set; }
        public string? Bio { get; set; }
    }
}
