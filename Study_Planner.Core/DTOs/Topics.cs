using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner.Core.DTOs
{
    public class Topics
    {
        public string TopicName { get; set; }
        public int OrderIndex { get; set; }
        public int? DifficultyLevel { get; set; } = 5;
        public int? EstimatedCompletionTime { get; set; } = 60;
        public List<SubTopics>? SubTopics { get; set; }
    }
}
