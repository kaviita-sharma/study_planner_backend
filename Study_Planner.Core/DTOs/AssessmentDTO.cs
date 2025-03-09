using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner.Core.DTOs
{
        public class AssessmentDTO
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int SubjectId { get; set; }
            public int? TopicId { get; set; }
            public string AssessmentName { get; set; }
            public string Description { get; set; }
            public string AssessmentType { get; set; }
            public decimal MaxScore { get; set; }
            public decimal? ActualScore { get; set; }
            public DateTime? CompletionDate { get; set; }
            public DateTime? DueDate { get; set; }
            public bool IsCompleted { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }
}

