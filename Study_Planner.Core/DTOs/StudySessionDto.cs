using System.Text.Json.Serialization;

namespace Study_Planner.Core.DTOs
{
        public class StudySessionDto
        {
            public int? Id { get; set; }
            public int UserId { get; set; }
            public string? Notes { get; set; }
            public DateTime ScheduledStartTime { get; set; }
            public DateTime ScheduledEndTime { get; set; }
            public DateTime? ActualStartTime { get; set; }
            public DateTime? ActualEndTime { get; set; }
            public string? status { get; set; }
            public int? FocusRating { get; set; }
            public int? ComprehensionRating { get; set; }
            public string? title {  get; set; }
        }
}

