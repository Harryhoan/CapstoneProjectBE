using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.ProjectDTO
{
    public class ProjectStatusDTO
    {
        public int projectId {  get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
    }
}
