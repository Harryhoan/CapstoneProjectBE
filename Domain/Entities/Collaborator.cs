using Domain.Enums;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Collaborator
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CollaboratorEnum Role { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual Project Project { get; set; } = null!;
    }
}
