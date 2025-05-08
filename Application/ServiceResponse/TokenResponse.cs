using System.Text.Json.Serialization;

namespace Application.ServiceResponse
{
    public class TokenResponse<T>
    {
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? DataToken { get; set; }
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Role { get; set; } = null;
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Avatar { get; set; } = null;
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? FullName { get; set; } = null;
        public bool Success { get; set; } = false;
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Message { get; set; } = null;
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Error { get; set; } = null;
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Hint { get; set; } = null;
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? HintId { get; set; } = null;
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Code { get; set; } = null;
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? ErrorMessages { get; set; } = null;
    }
}
