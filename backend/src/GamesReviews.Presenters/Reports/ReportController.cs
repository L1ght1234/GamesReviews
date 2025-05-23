﻿using GamesReviews.Application.Abstractions;
using GamesReviews.Application.AuthMethods;
using GamesReviews.Application.Comments.Features.GetCommentById;
using GamesReviews.Application.Reports.Features.CreateReport;
using GamesReviews.Application.Reports.Features.GetMyReport;
using GamesReviews.Application.Reports.Features.GetReport;
using GamesReviews.Application.Reports.Features.UpdateReportStatus;
using GamesReviews.Application.Reviews.Features.GetReviewById;
using GamesReviews.Contracts;
using GamesReviews.Contracts.Comments;
using GamesReviews.Contracts.Reports;
using GamesReviews.Contracts.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamesReviews.Presenters.Reports;

[ApiController]
[Route("/api/reports")]
public class ReportController : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateReport(
        [FromServices] ICommandHandler<Guid, CreateReportCommand> handler,
        [FromBody] CreateReportRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        
        var command = new CreateReportCommand(userId, request);
        
        var reportId = await handler.Handle(command, cancellationToken);
        
        return Ok(reportId);
    }

    [Authorize(Roles = "Moderator,Admin")]
    [HttpGet("moderation")]
    public async Task<IActionResult> GetReports(
        [FromQuery] GetReportsFilterRequest request,
        [FromServices] ICommandHandler<PagedResult<GetReportResponse>, GetReportCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new GetReportCommand(request);
        
        var reports = await handler.Handle(command, cancellationToken);
        
        return Ok(reports);
    }
    
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyReports(
        [FromQuery] GetReportsFilterRequest request,
        [FromServices] ICommandHandler<PagedResult<GetReportResponse>, GetMyReportCommand> handler,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        
        var command = new GetMyReportCommand(userId, request);
        
        var reports = await handler.Handle(command, cancellationToken);
        
        return Ok(reports);
    }

    [Authorize(Roles = "Moderator,Admin")]
    [HttpPut("moderation/{reportId:guid}")]
    public async Task<IActionResult> UpdateReportStatus(
        [FromRoute] Guid reportId,
        [FromBody] UpdateReportStatusRequest request,
        [FromServices] ICommandHandler<Guid, UpdateReportStatusCommand> handler,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        
        var command = new UpdateReportStatusCommand(reportId, userId, request);
        
        var updatedReportId = await handler.Handle(command, cancellationToken);
        
        return Ok(updatedReportId);
    }
    
    [HttpGet("content/{contentType:int}/{contentId:guid}")]
    public async Task<IActionResult> GetReportedContent(
        [FromRoute] int contentType,
        [FromRoute] Guid contentId,
        [FromServices] ICommandHandler<GetReviewResponse, GetReviewByIdCommand> reviewHandler,
        [FromServices] ICommandHandler<CommentResponse, GetCommentByIdCommand> commentHandler,
        CancellationToken cancellationToken)
    {
        return contentType switch
        {
            0 => Ok(await reviewHandler.Handle(new GetReviewByIdCommand(contentId), cancellationToken)),
            1 => Ok(await commentHandler.Handle(new GetCommentByIdCommand(contentId), cancellationToken)),
            _ => BadRequest("Invalid content type")
        };
    }
}