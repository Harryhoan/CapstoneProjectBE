﻿using Application.ViewModels.UserDTO;

namespace Application.ViewModels.CommentDTO
{
    public class CommentDTO
    {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDatetime { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public DateTime UpdatedDatetime { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public PostUserDTO User { get; set; } = new();
        public ICollection<CommentDTO> Comments { get; set; } = new List<CommentDTO>();

    }
}
