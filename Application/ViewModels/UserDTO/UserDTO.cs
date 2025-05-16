namespace Application.ViewModels.UserDTO
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string? Avatar { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? PaymentAccount { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public bool IsVerified { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDatetime { get; set; }
    }
}
