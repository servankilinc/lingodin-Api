using Core.Exceptions;
using Core.Exceptions.ProblemDetailModels;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace WebApi.GlobalExceptionHandler;

public class ExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ExceptionHandler> _logger;
    public ExceptionHandler(ILogger<ExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.ContentType = "application/json";
        Type typeExc = exception.GetType();

        string? exceptionDetail = "Empty Exception Detail...";
        if (typeof(ValidationException) == typeExc)
        {
            httpContext.Response.StatusCode = Convert.ToInt32(HttpStatusCode.BadRequest);
            exceptionDetail = CreateValidationException(exception);
        }
        else if (typeof(BusinessException) == typeExc)
        {
            httpContext.Response.StatusCode = Convert.ToInt32(HttpStatusCode.BadRequest);
            exceptionDetail = CreateBusinessException(exception);
        }
        else if (typeof(DataAccessException) == typeExc)
        {
            httpContext.Response.StatusCode = Convert.ToInt32(HttpStatusCode.InternalServerError);
            exceptionDetail = CreateDataAccessException(exception);
        }
        else
        {
            httpContext.Response.StatusCode = Convert.ToInt32(HttpStatusCode.InternalServerError);
            exceptionDetail = CreateInternalException(exception);
        }
        await httpContext.Response.WriteAsync(exceptionDetail);

        _logger.LogError($"Error => {exceptionDetail}");

        return true;
    }


    private string CreateValidationException(Exception exception)
    {
        IEnumerable<ValidationFailure> errors = ((ValidationException)exception).Errors;
        return new ValidationProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Type = ProblemDetailTypes.Validation.ToString(),
            Title = "Validation Error(s)",
            Detail = "",
            Errors = errors
        }.ToString();
    }
    private string CreateBusinessException(Exception exception)
    {
        return new BusinessProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Type = ProblemDetailTypes.Business.ToString(),
            Title = "Business Wrok Flow Excepiton",
            Detail = exception.Message,
        }.ToString();
    }
    private string CreateDataAccessException(Exception exception)
    {
        return new DataAccessProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = ProblemDetailTypes.DataAccess.ToString(),
            Title = "Data Access, An Exception Occurs During The Process",
            Detail = exception.Message,
        }.ToString();
    }
    private string CreateInternalException(Exception exception)
    {
        return JsonSerializer.Serialize(new Microsoft.AspNetCore.Mvc.ProblemDetails()
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = ProblemDetailTypes.General.ToString(),
            Detail = exception.Message,
            Title = "Internal exception",
        });
    }
}