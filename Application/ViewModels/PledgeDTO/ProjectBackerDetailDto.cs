﻿using Domain.Enums;

namespace Application.ViewModels.PledgeDTO
{
    public class ProjectBackerDetailDto
    {
        public decimal Amount { get; set; }
        public PledgeDetailEnum Status { get; set; }
        public DateTime CreatedDatetime { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);

    }
}
