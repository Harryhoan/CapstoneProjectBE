using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Platform
    {
        [Key]
        public int PlatformId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public virtual ICollection<ProjectPlatform> ProjectPlatforms { get; set; } = new List<ProjectPlatform>();
    }
}
