using FluentValidation;
using TaskBoard.Application.Requests;

public class CreateTaskValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Icon).NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
    }
}