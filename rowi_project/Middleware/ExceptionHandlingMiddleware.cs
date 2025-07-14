using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rowi_project.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace rowi_project.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred during request {RequestPath}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        var statusCode = HttpStatusCode.InternalServerError;
        string title = "An unhandled error occurred";
        string detail = "An unexpected error occurred. Please try again later.";
        string? fieldName = null;
        var traceId = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier;

        switch (exception)
        {
            case ValidationException validationEx:
                statusCode = HttpStatusCode.BadRequest; // 400
                title = "Validation Error";
                detail = validationEx.Message;
                break;

            case ArgumentException argEx:
                statusCode = HttpStatusCode.BadRequest; // 400
                title = "Invalid Argument";
                detail = argEx.Message;
                break;

            case NotFoundException notFoundEx:
                statusCode = HttpStatusCode.NotFound; // 404
                title = "Resource Not Found";
                detail = notFoundEx.Message;
                break;

            case DuplicateEntryException duplicateEx:
                statusCode = HttpStatusCode.Conflict; // 409
                title = "Duplicate Entry";
                detail = duplicateEx.Message;
                fieldName = duplicateEx.FieldName;
                break;

            case DbUpdateException:
                statusCode = HttpStatusCode.InternalServerError; // 500
                title = "Database Error";
                detail = "An unexpected database error occurred.";
                break;
        }
        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
            Instance = context.Request.Path
        };

        problemDetails.Extensions["traceId"] = traceId;
        
        if (!string.IsNullOrWhiteSpace(fieldName))
            problemDetails.Extensions["fieldName"] = fieldName;

        context.Response.StatusCode = (int)statusCode;
        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }
}
