namespace Application.ViewModels.RewardDTO
{
    public class AddReward
    {
        public int ProjectId { get; set; }
        public decimal Amount { get; set; }
        public string Details { get; set; } = string.Empty;
        public DateTime CreatedDatetime { get; set; }
    }
}
