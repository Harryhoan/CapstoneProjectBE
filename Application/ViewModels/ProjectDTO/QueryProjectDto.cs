﻿using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.ViewModels.ProjectDTO
{
    public class QueryProjectDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Creator ID must be a positive integer")]
        public int? CreatorId { get; set; } = null;
        [StringLength(50, ErrorMessage = "Title can't be longer than 50 characters")]
        public string? Title { get; set; } = null;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ProjectStatusEnum? ProjectStatus { get; set; } = null;
        //[JsonConverter(typeof(JsonStringEnumConverter))]
        //public TransactionStatusEnum? TransactionStatus { get; set; } = null;

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
        [DataType(DataType.DateTime, ErrorMessage = "Invalid data type for the point of time the Project was created")]
        public DateTime? MinCreatedDatetime { get; set; } = null;
        [DataType(DataType.DateTime, ErrorMessage = "Invalid data type for the point of time the Project was created")]
        public DateTime? MaxCreatedDatetime { get; set; } = null;
        [DataType(DataType.DateTime, ErrorMessage = "Invalid data type for the point of time the Project was last updated")]
        public DateTime? MinUpdatedDatetime { get; set; } = null;
        [DataType(DataType.DateTime, ErrorMessage = "Invalid data type for the point of time the Project was last updated")]
        public DateTime? MaxUpdatedDatetime { get; set; } = null;
        [DataType(DataType.DateTime, ErrorMessage = "Invalid data type for the point of time the Project ends")]
        public DateTime? MinEndDatetime { get; set; } = null;
        [DataType(DataType.DateTime, ErrorMessage = "Invalid data type for the point of time the Project ends")]
        public DateTime? MaxEndDatetime { get; set; } = null;
        public IList<int>? CategoryIds { get; set; } = null;
        public IList<int>? PlatformIds { get; set; } = null;
    }
}
