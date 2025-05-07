using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.FaqDTO
{
    public class FaqDto
    {
        [Required(ErrorMessage = "Question is required")]
        public string Question { get; set; } = string.Empty;
        [Required(ErrorMessage = "Answer is required")]
        public string Answer { get; set; } = string.Empty;
    }
}
