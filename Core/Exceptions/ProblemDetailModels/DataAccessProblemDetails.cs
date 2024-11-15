﻿using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Core.Exceptions.ProblemDetailModels;

public class DataAccessProblemDetails : ProblemDetails
{
    public override string ToString() => JsonSerializer.Serialize(this);
}
