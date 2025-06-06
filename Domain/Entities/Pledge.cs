﻿using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Pledge
    {
        [Key]
        public int PledgeId { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public decimal TotalAmount { get; set; }

        public virtual Project Project { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<PledgeDetail> PledgeDetails { get; set; } = new List<PledgeDetail>();
    }
}
