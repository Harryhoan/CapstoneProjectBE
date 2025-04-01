using Domain.Enums;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.ViewModels.CollaboratorDTO
{
    public class CreateCollaboratorDTO
    {
        [Required(ErrorMessage = "User ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Project ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Project ID must be a positive integer")]
        public int ProjectId { get; set; }
        [Required(ErrorMessage = "Role is required")]
        [EnumDataType(typeof(CollaboratorEnum), ErrorMessage = "Role must be ADMINISTRATOR, EDITOR or VIEWER")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CollaboratorEnum Role { get; set; }

    }
}
