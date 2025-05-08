using GamesReviews.Application.Exceptions;
using Shared;

namespace GamesReviews.Application.Reports.Exceptions;

public class ReportNotFoundException : NotFoundException
{
    public ReportNotFoundException(Error[] errors) : base(errors)
    {
    }
}