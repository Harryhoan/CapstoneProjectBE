﻿using Domain.Enums;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.ViewModels.PostDTO
{
    public class UpdatePostDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(50, ErrorMessage = "Title can't be longer than 50 characters")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Status is required")]
        [EnumDataType(typeof(PostEnum), ErrorMessage = "Post must be PRIVATE, DELETED, EXCLUSIVE or PUBLIC")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PostEnum Status { get; set; }
    }
}
