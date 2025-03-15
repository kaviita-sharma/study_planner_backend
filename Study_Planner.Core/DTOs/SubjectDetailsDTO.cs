

namespace Study_Planner.Core.DTOs
{
    public class SubjectDetailsDTO
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }     
        public string Priority { get; set; }        
        public string DifficultyLevel { get; set; } 
        public int? EstimatedCompletionHours { get; set; }
    }
}
