namespace Study_Planner.Core.DTOs
{
    using System;

    namespace Study_Planner.Core.DTOs
    {
        public class ProgressDTO
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int SubjectId { get; set; }
            public int? TopicId { get; set; }
            public int? SubTopicId { get; set; }
            public decimal CompletionPercentage { get; set; }
            public DateTime? LastStudyDate { get; set; }
            public DateTime? NextReviewDate { get; set; }
            public int? ConfidenceLevel { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }
    }

}
