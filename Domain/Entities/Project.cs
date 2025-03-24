using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Project
    {
        public int ProjectId { get; set; }
        public int CreatorId { get; set; }
        public string? Thumbnail {  get; set; }
        public int MonitorId { get; set; } // New field
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal MinimumAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime StartDatetime { get; set; }
        public DateTime UpdateDatetime { get; set; }
        public DateTime EndDatetime { get; set; }
        public virtual ICollection<ProjectComment> ProjectComments { get; set; } = new List<ProjectComment>();
        public virtual ICollection<Collaborator> Collaborators { get; set; } = new List<Collaborator>();
        public virtual ICollection<ProjectPlatform> ProjectPlatforms { get; set; } = new List<ProjectPlatform>();
        public virtual ICollection<ProjectCategory> ProjectCategories { get; set; } = new List<ProjectCategory>();
        public virtual ICollection<Goal> Goals { get; set; } = new List<Goal>();
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual User User { get; set; } = null!;
        public virtual User Monitor { get; set; } = null!; // New relationship
        public virtual ICollection<Pledge> Pledges  { get; set; } = new List<Pledge>();
        public virtual ICollection<Reward> Rewards { get; set; } = new List<Reward>();
    }
}
