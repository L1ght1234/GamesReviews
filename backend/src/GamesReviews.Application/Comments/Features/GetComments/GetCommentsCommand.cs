using GamesReviews.Application.Abstractions;
using GamesReviews.Contracts.Comments;

namespace GamesReviews.Application.Comments.Features.GetComments;

public record GetCommentsCommand(Guid ReviewId, GetCommentsRequest GetCommentsRequest, 
    Guid? ParentCommentId = null) : ICommand;