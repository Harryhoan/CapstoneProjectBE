using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }
        public int CreatorId { get; set; }
        public string? Thumbnail { get; set; }
        public int MonitorId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Story { get; set; }
        public string Description { get; set; } = string.Empty;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ProjectStatusEnum Status { get; set; }
        public TransactionStatusEnum TransactionStatus { get; set; }
        public decimal MinimumAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime StartDatetime { get; set; }
        public DateTime CreatedDatetime { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public DateTime UpdatedDatetime { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public DateTime EndDatetime { get; set; }
        public virtual ICollection<ProjectComment> ProjectComments { get; set; } = new List<ProjectComment>();
        public virtual ICollection<Collaborator> Collaborators { get; set; } = new List<Collaborator>();
        public virtual ICollection<ProjectPlatform> ProjectPlatforms { get; set; } = new List<ProjectPlatform>();
        public virtual ICollection<ProjectCategory> ProjectCategories { get; set; } = new List<ProjectCategory>();
        public virtual ICollection<FAQ> Questions { get; set; } = new List<FAQ>();
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual User User { get; set; } = null!;
        public virtual User Monitor { get; set; } = null!; // New relationship
        public virtual ICollection<Pledge> Pledges { get; set; } = new List<Pledge>();
        public virtual ICollection<Reward> Rewards { get; set; } = new List<Reward>();
    }
}
