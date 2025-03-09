using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner.Core.DTOs
{
    public class Subjects
    {
        public string SubjectName { get; set; }
        public int DifficultyLevel { get; set; }
        public int Priority { get; set; }
        public int? EstimatedCompletionTime { get; set; }
        public string Status { get; set; } = "ToDo";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Topics Topic { get; set; }
    }
}
