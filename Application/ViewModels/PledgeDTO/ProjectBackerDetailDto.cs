using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.PledgeDTO
{
    public class ProjectBackerDetailDto
    {
        public decimal Amount { get; set; }
        public PledgeDetailEnum Status { get; set; }
        public DateTime CreatedDatetime { get; set; }

    }
}
