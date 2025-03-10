namespace Study_Planner.Core.DTOs
{
    public class CreateAssessmentDTO
    {
        public int UserId { get; set; }
        public int SubjectId { get; set; }
        public int? TopicId { get; set; }
        public string AssessmentName { get; set; }
        public string Description { get; set; }
        public string AssessmentType { get; set; }
        public decimal MaxScore { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
