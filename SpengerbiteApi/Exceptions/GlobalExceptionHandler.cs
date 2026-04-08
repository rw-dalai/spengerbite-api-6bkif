// ══════════════════════════════════════════════════════════════
//  Global Exception Handler — Exception Handling Strategy
// ══════════════════════════════════════════════════════════════
//
//  This handler catches ALL unhandled exceptions and returns
//  RFC 9457 ProblemDetails JSON to the client.
//
// ──────────────────────────────────────────────────────────────
//  Which layer throws what, and what happens:
// ──────────────────────────────────────────────────────────────
//
//  Domain Layer
//    |
//    |  ArgumentException        guard clauses in constructors
//    |  DomainException          business rule violations
//    |
//  Service Layer
//    |
//    |  ServiceException         application level outcomes (NotFound, Forbidden, Conflict)
//    |
//  EF Core / Database
//    |
//    |  DbUpdateException        constraint violations (unique, FK)
//    |
//  GlobalExceptionHandler        catches all, returns ProblemDetails
//    |
//    v
//
//  Exception                     Status   Detail
//  ─────────────────────────────────────────────────────────────
//  ServiceException              mapped   ex.Message (service chose the status)
//  DomainException               400      ex.Message (e.g. "Cannot cancel a delivered order")
//  ArgumentException             400      ex.Message (e.g. "firstName cannot be null")
//  DbUpdateException + UNIQUE    409      "A conflicting record already exists."
//  DbUpdateException (other)     500      "A database error occurred."
//  Everything else               500      "An unexpected error occurred."
//  ─────────────────────────────────────────────────────────────
//
//  The service layer:
//    THROWS ServiceException for application outcomes (NotFound, Conflict, Forbidden)
//    LETS THROUGH DomainException and ArgumentException (already have meaningful messages)
//    Does NOT need try/catch on SaveChangesAsync (this handler is the safety net)
//    Can do AnyAsync pre checks for friendly error messages before saving
//
// ──────────────────────────────────────────────────────────────
//  Registered: builder.Services.AddExceptionHandler<T>()
//  Activated:  app.UseExceptionHandler()
//  Returns:    RFC 9457 ProblemDetails JSON
//    { "status": 404, "title": "Not Found", "detail": "Customer not found: 42" }
// ══════════════════════════════════════════════════════════════

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using SpengerbiteApi.Extensions;
using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Exceptions;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Map: exception to HTTP status code + client safe message
        var (statusCode, detail) = exception switch
        {
            // - ServiceException thrown by the service layer for application level outcomes
            //   (e.g. "Customer not found: 42", "Cart is empty")
            ServiceException ex => (ex.StatusCode, ex.Message),
            
            // - DomainException thrown by domain layer for business rule violations
            //   (e.g. "Cannot cancel a delivered order")
            // - ArgumentException thrown by guard clauses in domain layer
            //   (e.g. "firstName cannot be null")
            DomainException ex => (400, ex.Message),
            ArgumentException ex => (400, ex.Message),

            // - DbUpdateException is thrown by EF Core when SaveChangesAsync for a constraint violation
            //  (e.g. unique constraint, FK constraint)
            DbUpdateException ex when ex.IsUniqueConstraintViolation() => 
                (409, "A conflicting record already exists."),
            
            // - Everything else unhandled is 500, but we don't want to leak details to the client
            _ => (500, "An unexpected error occurred.")
        };

        // Log: warnings for client errors (4xx), errors for server errors (5xx)
        var logLevel = statusCode >= 500 ? LogLevel.Error : LogLevel.Warning;

        logger.Log(logLevel, exception, "{StatusCode} {Path}", statusCode, httpContext.Request.Path);

        // Return: RFC 9457 ProblemDetails JSON
        httpContext.Response.StatusCode = statusCode;
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = ReasonPhrases.GetReasonPhrase(statusCode),
            Detail = detail
        };
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true; // true= exception was handled
    }
}