using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.RewardDTO
{
    public class AddReward
    {
        [Required(ErrorMessage = "Project ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Project ID must be a positive integer")]
        public int ProjectId { get; set; }
        [Required(ErrorMessage = "Amount cannot be empty")]
        [Range(0.01, 1000000, ErrorMessage = "Amount cannot be smaller or equal to 0")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Details")]
        public string Details { get; set; } = string.Empty;
    }
}
