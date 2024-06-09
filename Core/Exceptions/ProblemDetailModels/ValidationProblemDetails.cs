using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Core.Exceptions.ProblemDetailModels;

public class ValidationProblemDetails : ProblemDetails
{
    public IEnumerable<ValidationFailure>? Errors { get; set; }

    public override string ToString() => JsonSerializer.Serialize(this);
}