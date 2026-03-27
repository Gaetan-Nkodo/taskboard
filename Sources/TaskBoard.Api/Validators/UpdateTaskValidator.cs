using FluentValidation;
using TaskBoard.Application.Requests;

public class UpdateTaskValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Icon).NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
    }
}
