// ══════════════════════════════════════════════════════════════
//  Exception Handling in ASP.NET Core — Quick Reference
// ══════════════════════════════════════════════════════════════
//
//  The Problem: How to return proper HTTP error responses from the service layer?
//    The service layer doesn't know about HTTP (no access to HttpContext).
//    But it knows when something goes wrong (customer not found, cart empty, etc.)
//
//  The Solution: Throw a ServiceException, let the GlobalExceptionHandler
//    handle the HTTP error response as Problem Details (RFC 9457).
//
//  Flow:
//    1. Service throws ServiceException.NotFound($"Customer not found: {id}")
//    2. Exception bubbles up through the controller (no try/catch needed)
//    3. GlobalExceptionHandler catches it
//    4. Returns RFC 9457 ProblemDetails JSON:
//       { "status": 404, "title": "Not Found", "detail": "Customer not found: 42" }
//
// ──────────────────────────────────────────────────────────────
//  Pattern: Private constructor + static factory methods
// ──────────────────────────────────────────────────────────────
//
//  Why private constructor?
//    Forces callers to use the named factories (NotFound, Conflict, Forbidden, BadRequest)
//    which are self documenting. You can't accidentally create a wrong status code.
//
//  throw ServiceException.NotFound("...")    -> 404
//  throw ServiceException.Conflict("...")    -> 409
//  throw ServiceException.Forbidden("...")   -> 403
//  throw ServiceException.BadRequest("...")  -> 400
//
// ──────────────────────────────────────────────────────────────
//  RFC 9457: Problem Details for HTTP APIs
// ──────────────────────────────────────────────────────────────
//
//  Standard JSON format for error responses (used by ProblemDetails class):
//    {
//      "status": 404,                          // HTTP status code
//      "title": "Not Found",                   // short description (from ReasonPhrases)
//      "detail": "Customer not found: 42"      // specific error message
//    }
//
//  See: https://www.rfc-editor.org/rfc/rfc9457
// ══════════════════════════════════════════════════════════════

namespace SpengerbiteApi.Exceptions;

public class ServiceException : Exception
{
    public int StatusCode { get; }

    private ServiceException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }

    public static ServiceException NotFound(string message) => 
        new(StatusCodes.Status404NotFound, message);

    public static ServiceException Forbidden(string message) => 
        new(StatusCodes.Status403Forbidden, message);

    public static ServiceException Conflict(string message) => 
        new(StatusCodes.Status409Conflict, message);

    public static ServiceException BadRequest(string message) => 
        new(StatusCodes.Status400BadRequest, message);
}
