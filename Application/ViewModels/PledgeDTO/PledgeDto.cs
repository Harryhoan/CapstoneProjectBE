namespace Application.ViewModels.PledgeDTO
{
    public class PledgeDto
    {
        public int PledgeId { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PledgeDetailDto> PledgeDetails { get; set; } = new();
    }
}
