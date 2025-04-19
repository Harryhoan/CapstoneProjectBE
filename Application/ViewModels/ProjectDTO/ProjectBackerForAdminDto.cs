using Application.ViewModels.PledgeDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.ProjectDTO
{
    public class ProjectBackerForAdminDto
    {
        public int BackerId { get; set; }
        public string BackerName { get; set; } = string.Empty;
        public string BackerAvatar { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<PledgeDetailDto> PledgeDetailDtos { get; set; } = new List<PledgeDetailDto>();
    }
}
