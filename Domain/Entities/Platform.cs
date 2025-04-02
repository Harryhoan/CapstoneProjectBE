namespace Domain.Entities
{
    public class Platform
    {
        public int PlatformId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public virtual ICollection<ProjectPlatform> GamePlatforms { get; set; } = new List<ProjectPlatform>();
    }
}
