using FluentValidation;
using TaskBoard.Application.Requests;

public class CreateBoardValidator : AbstractValidator<CreateBoardRequest>
{
    public CreateBoardValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

