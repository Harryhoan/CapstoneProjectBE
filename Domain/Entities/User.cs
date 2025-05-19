using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Domain.Entities
{
    public class User
    {
        private string? _phone;
        private string? _paymentAccount = string.Empty;
        [Key]
        public int UserId { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? PaymentAccount
        {
            get => _paymentAccount;
            set
            {
                _paymentAccount = value;
                VerifyUser();
            }
        }
        public string? Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                VerifyUser();
            }
        }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserEnum Role { get; set; }
        public string Bio { get; set; } = string.Empty;
        public bool IsVerified { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDatetime { get; set; } = DateTime.Now;

        // Relationships
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<File> Files { get; set; } = new List<File>();
        public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
        public virtual ICollection<Pledge> Pledges { get; set; } = new List<Pledge>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Collaborator> Collaborators { get; set; } = new List<Collaborator>();
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<Project> MonitoredProjects { get; set; } = new List<Project>(); // New relationship
        private void VerifyUser()
        {
            if (!string.IsNullOrWhiteSpace(_phone) && !string.IsNullOrWhiteSpace(_paymentAccount) && Regex.IsMatch(_phone, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,15}$") && new EmailAddressAttribute().IsValid(_paymentAccount))
            {
                IsVerified = true;
            }
        }
    }
}
