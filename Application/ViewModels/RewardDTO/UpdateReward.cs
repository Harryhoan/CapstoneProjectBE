using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.RewardDTO
{
    public class UpdateReward
    {
        [Required(ErrorMessage = "Amount cannot be empty")]
        [Range(0.01, 1000000, ErrorMessage = "Amount cannot be smaller or equal to 0")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Details")]
        public string Details { get; set; } = string.Empty;
    }
}
