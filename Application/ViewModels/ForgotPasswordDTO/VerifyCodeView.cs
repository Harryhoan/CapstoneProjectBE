using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.ForgotPasswordDTO
{
    public class VerifyCodeView
    {
        public required string Name { get; set; }
        public required DateTime CreateAt { get; set; }
    }
}
