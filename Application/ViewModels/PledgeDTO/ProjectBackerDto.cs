using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.PledgeDTO
{
    public class ProjectBackerDto
    {
        public int BackerId { get; set; }
        public string BackerName { get; set; } = string.Empty;
        public string BackerAvatar { get; set; } = string.Empty;
        //public PledgeDto pledge { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public List<ProjectBackerDetailDto> ProjectBackerDetails { get; set; } = new List<ProjectBackerDetailDto>();
    }
}
