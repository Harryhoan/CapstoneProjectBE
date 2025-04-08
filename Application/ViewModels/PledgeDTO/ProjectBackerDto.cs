using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.PledgeDTO
{
    public class ProjectBackerDto
    {
        public int? backerId { get; set; }
        public string backerName { get; set; } = string.Empty;
        public string backerAvatar { get; set; } = string.Empty;
        public PledgeDto pledge { get; set; } = null!;
    }
}
