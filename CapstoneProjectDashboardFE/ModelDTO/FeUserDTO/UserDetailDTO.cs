using Newtonsoft.Json;

namespace CapstoneProjectDashboardFE.ModelDTO.FeUserDTO
{
    public class UserDetailDTO
    {
        [JsonProperty("user-id")] // Ensure the correct JSON key
        public int UserId { get; set; }
        [JsonProperty("full-name")]
        public string Fullname { get; set; } = string.Empty;
        [JsonProperty("avatar")]
        public string? Avatar { get; set; }
        [JsonProperty("Email")]
        public string Email { get; set; } = string.Empty;
        [JsonProperty("payment-account")]
        public string PaymentAccount { get; set; } = string.Empty;
        [JsonProperty("phone")]
        public string? Phone { get; set; }
        [JsonProperty("role")]
        public string Role { get; set; } = string.Empty;
        [JsonProperty("Bio")]
        public string Bio { get; set; } = string.Empty;
        [JsonProperty("create-datetime")]
        public DateTime CreatedDatetime { get; set; }
    }
}
