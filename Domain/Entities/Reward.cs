namespace Domain.Entities
{
    public class Reward
    {
        public int RewardId { get; set; }
        public int ProjectId { get; set; }
        public decimal Amount { get; set; }
        public string Details { get; set; } = string.Empty;
        public DateTime CreatedDatetime { get; set; }

        public virtual Project Project { get; set; } = null!;
    }
}
