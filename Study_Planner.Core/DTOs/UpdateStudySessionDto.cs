public class UpdateStudySessionDto
{
    public int? SubjectId { get; set; }
    public int? TopicId { get; set; }
    public int? SubTopicId { get; set; }
    public string? Notes { get; set; }
    public DateTime? ScheduledStartTime { get; set; }
    public DateTime? ScheduledEndTime { get; set; }
    public string? Status { get; set; }
    public int? FocusRating { get; set; }
    public int? ComprehensionRating { get; set; }
}
