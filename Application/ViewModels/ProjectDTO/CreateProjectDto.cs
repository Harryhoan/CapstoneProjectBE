using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.ProjectDTO
{
    public class CreateProjectDto
    {
        public int CreatorId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Status Status { get; set; }
        public decimal MinimumAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime StartDatetime { get; set; }
        public DateTime EndDatetime { get; set; }
    }

    public enum Status
    {
        Ongoing,
        Halted,
        Invisible,
        Deleted
    }
}
