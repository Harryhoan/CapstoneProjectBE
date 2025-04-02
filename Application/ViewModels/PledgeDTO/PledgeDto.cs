namespace Application.ViewModels.PledgeDTO
{
    public class PledgeDto
    {
        public int PledgeId { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public decimal Amount { get; set; }
        public List<PledgeDetailDto> pledgeDetail { get; set; } = null!;
    }
}
