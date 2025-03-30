using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.PlatformDTO
{
    public class ViewPlatformDto
    {
        public int PlatformId { get; set; }
        public int ProjectId { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public string Monitor { get; set; } = string.Empty;
        public int CreatorId { get; set; }
        public string Creator { get; set; } = string.Empty;
        public int Backers { get; set; } = 0;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ProjectEnum Status { get; set; }
        public decimal MinimumAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime StartDatetime { get; set; }
        public DateTime EndDatetime { get; set; }
    }
}
