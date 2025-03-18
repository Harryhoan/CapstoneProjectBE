using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.PledgeDTO
{
    public class PledgeDto
    {
        public int PledgeId { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public decimal Amount { get; set; }
    }
}
