using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner.Core.DTOs
{
    public class ProgressWithDetailsDTO
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int Priority { get; set; }
        public int DifficultyLevel { get; set; }
        public int? EstimatedCompletionTime { get; set; }
    }
}
