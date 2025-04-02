namespace Application.ViewModels.RewardDTO
{
    public class ViewReward
    {
        public int RewardId { get; set; }
        public int ProjectId { get; set; }
        public decimal Amount { get; set; }
        public string Details { get; set; } = string.Empty;
        public DateTime CreatedDatetime { get; set; }
    }
}
