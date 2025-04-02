namespace Application.ViewModels.RewardDTO
{
    public class UpdateReward
    {
        public decimal Amount { get; set; }
        public string Details { get; set; } = string.Empty;
        public DateTime CreatedDatetime { get; set; }
    }
}
