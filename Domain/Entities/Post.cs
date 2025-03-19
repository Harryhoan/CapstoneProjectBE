using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Post
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public DateTime CreatedDatetime { get; set; }

        // Relationships
        public virtual User User { get; set; } = null!;
        public virtual Project Project { get; set; } = null!;
        public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();

    }
}
