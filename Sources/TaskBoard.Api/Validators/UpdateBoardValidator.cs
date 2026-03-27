using FluentValidation;
using TaskBoard.Application.Requests;

public class UpdateBoardValidator : AbstractValidator<UpdateBoardRequest>
{
    public UpdateBoardValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}