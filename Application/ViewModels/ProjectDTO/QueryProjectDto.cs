using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.ViewModels.ProjectDTO
{
    public class QueryProjectDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Creator ID must be a positive integer")]
        public int? CreatorId { get; set; } = null;
        [StringLength(50, ErrorMessage = "Title can't be longer than 50 characters")]
        public string? Title { get; set; } = null;
        //public string? Description { get; set; } = null;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        //[EnumDataType(typeof(ProjectEnum?), ErrorMessage = "Status must be ONGOING, INVISIBLE, HALTED or DELETED")]
        public ProjectStatusEnum? Status { get; set; } = null;
        [Range(0, 10000000, ErrorMessage = "The queryable Minimum Amount of a Project must be 0 or above")]
        public decimal? MinMinimumAmount { get; set; } = null;
        [Range(0, 10000000, ErrorMessage = "The queryable Minimum Amount of a Project must be 0 or above")]
        public decimal? MaxMinimumAmount { get; set; } = null;
        [Range(0, 10000000, ErrorMessage = "The queryable Total Amount of a Project must be 0 or above")]
        public decimal? MinTotalAmount { get; set; } = null;
        [Range(0, 10000000, ErrorMessage = "The queryable Total Amount of a Project must be 0 or above")]
        public decimal? MaxTotalAmount { get; set; } = null;
        [DataType(DataType.DateTime, ErrorMessage = "Invalid data type for the point of time the Project starts")]
        public DateTime? MinStartDatetime { get; set; } = null;
        [DataType(DataType.DateTime, ErrorMessage = "Invalid data type for the point of time the Project starts")]
        public DateTime? MaxStartDatetime { get; set; } = null;
        [DataType(DataType.DateTime, ErrorMessage = "Invalid data type for the point of time the Project is last updated")]
        public DateTime? MinUpdateDatetime { get; set; } = null;
        [DataType(DataType.DateTime, ErrorMessage = "Invalid data type for the point of time the Project is last updated")]
        public DateTime? MaxUpdateDatetime { get; set; } = null;
        [DataType(DataType.DateTime, ErrorMessage = "Invalid data type for the point of time the Project ends")]
        public DateTime? MinEndDatetime { get; set; } = null;
        [DataType(DataType.DateTime, ErrorMessage = "Invalid data type for the point of time the Project ends")]
        public DateTime? MaxEndDatetime { get; set; } = null;
        public IList<int>? CategoryIds { get; set; } = null;
        public IList<int>? PlatformIds { get; set; } = null;
    }
}
