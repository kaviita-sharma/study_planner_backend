namespace Study_Planner.Core.DTOs
{
    public class UpdateAssessmentDTO
    {
        public string? AssessmentName { get; set; }
        public string? Description { get; set; }
        public string? AssessmentType { get; set; }
        public decimal? MaxScore { get; set; }
        public decimal? ActualScore { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime? DueDate { get; set; }
        public bool? IsCompleted { get; set; }
    }
}
