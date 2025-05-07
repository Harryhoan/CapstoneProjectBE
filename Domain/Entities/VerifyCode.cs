namespace Domain.Entities
{
    public class VerifyCode
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public bool IsVerified { get; set; } = false;

    }
}
