using FluentValidation;
using Study_Planner.Core.DTOs;
using System;
namespace StudyPlanner.Application.Validators
{
    public class UpdateAssessmentValidator : AbstractValidator<UpdateAssessmentDTO>
    {
        public UpdateAssessmentValidator()
        {
            RuleFor(x => x.AssessmentName)
                .MaximumLength(100).WithMessage("Assessment Name cannot exceed 100 characters.")
                .When(x => x.AssessmentName != null);

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .When(x => x.Description != null);

            RuleFor(x => x.AssessmentType)
                .Must(type => new[] { "Quiz", "Test", "Exam", "Practice" }.Contains(type))
                .WithMessage("AssessmentType must be one of: Quiz, Test, Exam, Practice.")
                .When(x => x.AssessmentType != null);

            RuleFor(x => x.MaxScore)
                .GreaterThan(0).WithMessage("MaxScore must be greater than 0.")
                .When(x => x.MaxScore.HasValue);

            RuleFor(x => x.ActualScore)
                .GreaterThanOrEqualTo(0).WithMessage("ActualScore must be 0 or more.")
                .When(x => x.ActualScore.HasValue);

            RuleFor(x => x.CompletionDate)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("CompletionDate cannot be in the future.")
                .When(x => x.CompletionDate.HasValue);

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("DueDate must be in the future.")
                .When(x => x.DueDate.HasValue);
        }
    }
}