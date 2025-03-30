namespace CapstoneProjectDashboardFE.ModelDTO.FeUserDTO
{
    public class CreateStaffRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
