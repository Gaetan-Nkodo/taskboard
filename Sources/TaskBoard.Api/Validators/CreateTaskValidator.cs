using FluentValidation;
using TaskBoard.Application.Requests;

public class CreateTaskValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskValidator()
    {
        RuleFor(x => x.ColumnId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Icon).MaximumLength(10);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}
