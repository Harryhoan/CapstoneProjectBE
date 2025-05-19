namespace Application.ViewModels.FileDTO
{
    public class FileDTO
    {
        public int FileId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime CreatedDatetime { get; set; } = DateTime.Now;

    }
}
