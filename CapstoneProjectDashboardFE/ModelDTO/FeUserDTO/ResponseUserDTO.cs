using Newtonsoft.Json;

namespace CapstoneProjectDashboardFE.ModelDTO.FeUserDTO
{
    public class ResponseUserDTO
    {
        [JsonProperty("user-id")] // Ensure the correct JSON key
        public int UserId { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("phone")]
        public string Phone { get; set; } = string.Empty;

        [JsonProperty("full-name")] // Ensure correct mapping
        public string FullName { get; set; } = string.Empty;

        [JsonProperty("role")]
        public string Role { get; set; } = string.Empty;

        [JsonProperty("bio")]
        public string Bio { get; set; } = string.Empty;
    }
}
