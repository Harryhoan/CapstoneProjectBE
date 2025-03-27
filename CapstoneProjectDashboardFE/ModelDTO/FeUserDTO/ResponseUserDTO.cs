using Newtonsoft.Json;

namespace CapstoneProjectDashboardFE.ModelDTO.FeUserDTO
{
    public class ResponseUserDTO
    {
        [JsonProperty("user-id")] // Ensure the correct JSON key
        public int UserId { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("full-name")] // Ensure correct mapping
        public string FullName { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("bio")]
        public string Bio { get; set; }
    }
}
