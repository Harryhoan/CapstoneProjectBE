using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.FileDTO
{
    public class FileDTO
    {
        public int FileId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}
