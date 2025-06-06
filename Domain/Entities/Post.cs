﻿using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PostEnum Status { get; set; }

        public DateTime CreatedDatetime { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);

        // Relationships
        public virtual User User { get; set; } = null!;
        public virtual Project Project { get; set; } = null!;
        public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();

    }
}
