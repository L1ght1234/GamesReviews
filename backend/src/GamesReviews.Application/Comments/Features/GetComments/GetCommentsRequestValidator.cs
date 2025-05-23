﻿using FluentValidation;
using GamesReviews.Contracts.Comments;

namespace GamesReviews.Application.Comments.Features.GetComments;

public class GetCommentsRequestValidator : AbstractValidator<GetCommentsRequest>
{
    public GetCommentsRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0.");
    }
}