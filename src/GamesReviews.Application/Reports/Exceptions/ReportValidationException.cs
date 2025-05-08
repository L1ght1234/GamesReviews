using GamesReviews.Application.Exceptions;
using Shared;

namespace GamesReviews.Application.Reports.Exceptions;

public class ReportValidationException : BadRequestException
{
    public ReportValidationException(Error[] errors) : base(errors)
    {
    }
}