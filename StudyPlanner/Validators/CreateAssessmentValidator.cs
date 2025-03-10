using FluentValidation;
using Study_Planner.Core.DTOs;
using System;
namespace StudyPlanner.Application.Validators
{
    public class CreateAssessmentValidator : AbstractValidator<CreateAssessmentDTO>
    {
        public CreateAssessmentValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");

            RuleFor(x => x.SubjectId)
                .GreaterThan(0).WithMessage("SubjectId must be greater than 0.");

            RuleFor(x => x.AssessmentName)
                .NotEmpty().WithMessage("Assessment Name is required.")
                .MaximumLength(100).WithMessage("Assessment Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.AssessmentType)
                .NotEmpty().WithMessage("Assessment Type is required.")
                .Must(type => new[] { "Quiz", "Test", "Exam", "Practice" }.Contains(type))
                .WithMessage("AssessmentType must be one of: Quiz, Test, Exam, Practice.");

            RuleFor(x => x.MaxScore)
                .GreaterThan(0).WithMessage("MaxScore must be greater than 0.");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("DueDate must be in the future.")
                .When(x => x.DueDate.HasValue);
        }
    }
}