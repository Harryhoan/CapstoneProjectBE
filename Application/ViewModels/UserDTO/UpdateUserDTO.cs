namespace Application.ViewModels.UserDTO
{
    public class UpdateUserDTO
    {
        public string? Fullname { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? PaymentAccount { get; set; }
        public string? Password { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? Bio { get; set; } = string.Empty;
    }
}
