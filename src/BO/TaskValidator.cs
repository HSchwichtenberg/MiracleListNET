using System;
using FluentValidation;
using ITVisions;

namespace BO
{
 /// <summary>
 /// Validierung der Task-Objekte via Fluent Validation
 /// https://docs.fluentvalidation.net
 /// wird verwendet im UI, z.B. im Blazor-UI in TaskEdit.razor
 /// </summary>
 public class TaskValidator : AbstractValidator<BO.Task>
 {
  public TaskValidator()
  {
   RuleFor(t => t.Title).NotNull().MinimumLength(1).WithMessage("Title must be at least 1 character.");
   RuleFor(t => t.Due).Must(x => !x.HasValue || (x.HasValue && x.Value >= DateTime.Now.StartOfDay())).WithMessage("Due date must not be in the past");
   RuleFor(t => t.Effort).GreaterThan(0).WithMessage("Effort must be greater than 0 if not empty");
  }
 }
}