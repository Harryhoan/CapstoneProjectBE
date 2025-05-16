using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.UserDTO
{
    public class VerifyUserDTO
    {
        [EmailAddress(ErrorMessage = "Invalid payment account")]
        public string PaymentAccount { get; set; } = string.Empty;
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits long and contain only numbers")]
        public string Phone { get; set; } = string.Empty;

    }
}
