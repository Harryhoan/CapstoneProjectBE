using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.ProjectDTO
{
    public class CreateProjectDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal MinimumAmount { get; set; }
        public DateTime StartDatetime { get; set; }
        public DateTime EndDatetime { get; set; }
    }
}
