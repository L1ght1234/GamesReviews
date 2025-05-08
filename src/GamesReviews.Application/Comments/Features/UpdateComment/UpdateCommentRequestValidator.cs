using FluentValidation;
using GamesReviews.Contracts.Comments;

namespace GamesReviews.Application.Comments.Features.UpdateComment;

public class UpdateCommentRequestValidator : AbstractValidator<UpdateCommentRequest>
{
    public UpdateCommentRequestValidator()
    {
        RuleFor(x => x.Text)
            .MaximumLength(505).WithMessage("Text name must not exceed 505 characters");
    }
}