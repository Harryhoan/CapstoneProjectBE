using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.ProjectDTO
{
    public class UserProjectsDto
    {
        public int ProjectId { get; set; }
        public string? Title { get; set; }
        public string? Thumbnail { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime EndDatetime { get; set; }
        public ProjectEnum Status { get; set; }
    }
}
