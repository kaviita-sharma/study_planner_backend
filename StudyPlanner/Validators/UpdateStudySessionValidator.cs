using FluentValidation;

public class UpdateStudySessionValidator : AbstractValidator<UpdateStudySessionDto>
{
    public UpdateStudySessionValidator()
    {
        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");

        RuleFor(x => x.ScheduledStartTime)
            .GreaterThan(DateTime.Now).When(x => x.ScheduledStartTime.HasValue)
            .WithMessage("Scheduled start time must be in the future.");

        RuleFor(x => x.ScheduledEndTime)
            .GreaterThan(x => x.ScheduledStartTime).When(x => x.ScheduledEndTime.HasValue)
            .WithMessage("End time must be after start time.");

        RuleFor(x => x.Status)
            .Must(status => new[] { "Scheduled", "Completed", "Cancelled", "Missed" }.Contains(status))
            .When(x => x.Status != null)
            .WithMessage("Invalid status. Accepted values: Scheduled, Completed, Cancelled, Missed.");

        RuleFor(x => x.FocusRating)
            .InclusiveBetween(1, 10).When(x => x.FocusRating.HasValue)
            .WithMessage("Focus rating must be between 1 and 10.");

        RuleFor(x => x.ComprehensionRating)
            .InclusiveBetween(1, 10).When(x => x.ComprehensionRating.HasValue)
            .WithMessage("Comprehension rating must be between 1 and 10.");
    }
}
