using FluentValidation;

public class CreateStudySessionValidator : AbstractValidator<CreateStudySessionDto>
{
    public CreateStudySessionValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than zero.");

        RuleFor(x => x.Notes)
            .NotEmpty().WithMessage("Notes are required.")
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");

        RuleFor(x => x.ScheduledStartTime)
            .NotEmpty().WithMessage("Scheduled start time is required.")
            .GreaterThan(DateTime.Now).WithMessage("Scheduled start time must be in the future.");

        RuleFor(x => x.ScheduledEndTime)
            .GreaterThan(x => x.ScheduledStartTime).WithMessage("End time must be after start time.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(status => new[] { "Scheduled", "Completed", "Cancelled", "Missed" }.Contains(status))
            .WithMessage("Invalid status. Accepted values: Scheduled, Completed, Cancelled, Missed.");

        RuleFor(x => x.FocusRating)
            .InclusiveBetween(1, 10).When(x => x.FocusRating.HasValue)
            .WithMessage("Focus rating must be between 1 and 10.");

        RuleFor(x => x.ComprehensionRating)
            .InclusiveBetween(1, 10).When(x => x.ComprehensionRating.HasValue)
            .WithMessage("Comprehension rating must be between 1 and 10.");
    }
}

