using FluentValidation;
using GamesReviews.Contracts.Comments;

namespace GamesReviews.Application.Comments.Features.CreateComment;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Comment text cannot be empty")
            .MaximumLength(505).WithMessage("Comment text cannot exceed 505 characters");        
    }
}