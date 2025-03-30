using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceResponse
{
    public class TokenResponse<T>
    {
        public T DataToken { get; set; }
        public string? Role { get; set; } = null;
        public string? Avatar { get; set; } = null;
        public bool Success { get; set; } = true;
        public string? Message { get; set; } = null;
        public string? Error { get; set; } = null;
        public string? Hint { get; set; } = null;
        public int? HintId { get; set; } = null;
        public string? Code { get; set; } = null;
        public List<string>? ErrorMessages { get; set; } = null;
    }
}
