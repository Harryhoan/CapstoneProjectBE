﻿using Newtonsoft.Json;

namespace CapstoneProjectDashboardFE.ModelDTO.FeProjectDTO
{
    public class FaqDTO
    {
        [JsonProperty("question")]
        public string Question { get; set; } = null!;
        [JsonProperty("answer")]
        public string Answer { get; set; } = null!;
        [JsonProperty("createdDatetime")]
        public DateTime CreatedDatetime { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        [JsonProperty("updatedDatetime")]
        public DateTime UpdatedDatetime { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
    }
}
