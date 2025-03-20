using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.PledgeDTO
{
    public class PledgeDetailDto
    {
        public int PledgeId { get; set; }
        public string PaymentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
