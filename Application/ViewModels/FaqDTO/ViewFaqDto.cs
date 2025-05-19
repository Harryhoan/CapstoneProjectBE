using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.FaqDTO
{
    public class ViewFaqDto
    {
        public int ProjectId { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public DateTime CreatedDatetime { get; set; } = DateTime.Now;
        public DateTime UpdatedDatetime { get; set; } = DateTime.Now;

    }
}
