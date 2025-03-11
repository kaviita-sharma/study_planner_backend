using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Study_Planner.Core.DTOs
{
    public class Topics
    {
        public int? TopicId { get; set; }
        public string? TopicName { get; set; }
        public int? SubjectId { get; set; }
        public int? OrderIndex { get; set; }
        public int? DifficultyLevel { get; set; } = 5;
        public int? EstimatedCompletionTime { get; set; } = 60;
        public bool? IsActive { get; set; }
        public List<SubTopics>? SubTopics { get; set; }
    }
}
