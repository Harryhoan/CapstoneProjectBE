using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
