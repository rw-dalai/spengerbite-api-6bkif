using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SpengerbiteApi.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        // Return RFC 9457 Problem Details response
        var problem = new ProblemDetails()
        {
            Detail = exception.Message,
            Status = StatusCodes.Status400BadRequest,

        };
            
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}