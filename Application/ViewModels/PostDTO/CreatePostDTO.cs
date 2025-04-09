using Domain.Enums;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.ViewModels.PostDTO
{
    public class CreatePostDTO
    {
        [Required(ErrorMessage = "Project ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Project ID must be a positive integer")]
        public int ProjectId { get; set; }
        [Required(ErrorMessage = "Title is required")]
        [StringLength(50, ErrorMessage = "Title can't be longer than 50 characters")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Status is required")]
        //[RegularExpression("^(Deleted|Private|Exclusive|Public)$", ErrorMessage = "Status can only be \"Deleted\", \"Private\", \"Exclusive\" or \"Public\".")] 
        [EnumDataType(typeof(CollaboratorEnum), ErrorMessage = "Role must be ADMINISTRATOR, EDITOR or VIEWER")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PostEnum Status { get; set; }

    }
}
