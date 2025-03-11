using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner.Core.DTOs
{
    public class SubTopics
    {
        public int? id {  get; set; }
        public string? SubTopicName { get; set; }
        public int? TopicId { get; set; }
        public int? DifficultyLevel { get; set; } = 5;
        public int? EstimatedCompletionTime { get; set; } = 60;
        public int? OrderIndex { get; set; }
        public bool? IsActive { get; set; }
    }
}

