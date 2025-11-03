using System.Net;
using System.Text.Json;
using AeInfinity.Domain.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace AeInfinity.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            Message = exception.Message,
            StatusCode = (int)HttpStatusCode.InternalServerError
        };

        switch (exception)
        {
            case FluentValidation.ValidationException fluentValidationException:
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "One or more validation errors occurred.";
                errorResponse.Errors = fluentValidationException.Errors
                    .Select(e => new ValidationError
                    {
                        Property = e.PropertyName,
                        Message = e.ErrorMessage
                    })
                    .ToList();
                break;

            case AeInfinity.Domain.Exceptions.ValidationException domainValidationException:
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = domainValidationException.Message;
                errorResponse.Errors = domainValidationException.Errors
                    .SelectMany(e => e.Value.Select(errorMessage => new ValidationError
                    {
                        Property = e.Key,
                        Message = errorMessage
                    }))
                    .ToList();
                break;

            case NotFoundException:
                errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            case UnauthorizedException:
                errorResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;

            case ForbiddenException:
                errorResponse.StatusCode = (int)HttpStatusCode.Forbidden;
                break;

            case DomainException:
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case DbUpdateException dbUpdateEx when dbUpdateEx.InnerException is SqliteException sqliteEx:
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = GetFriendlyDatabaseErrorMessage(sqliteEx);
                errorResponse.Errors = new List<ValidationError>
                {
                    new ValidationError
                    {
                        Property = "Database",
                        Message = GetFriendlyDatabaseErrorMessage(sqliteEx)
                    }
                };
                break;

            default:
                errorResponse.Message = "An internal server error occurred.";
                break;
        }

        response.StatusCode = errorResponse.StatusCode;
        await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }

    private static string GetFriendlyDatabaseErrorMessage(SqliteException sqliteException)
    {
        // SQLite Error 19 is constraint violation
        if (sqliteException.SqliteErrorCode == 19)
        {
            var message = sqliteException.Message.ToLower();
            
            if (message.Contains("foreign key"))
            {
                if (message.Contains("categoryid") || message.Contains("category_id"))
                {
                    return "The specified category does not exist. Please provide a valid category ID.";
                }
                if (message.Contains("listid") || message.Contains("list_id"))
                {
                    return "The specified list does not exist.";
                }
                if (message.Contains("userid") || message.Contains("user_id"))
                {
                    return "The specified user does not exist.";
                }
                return "A referenced record does not exist. Please check your input values.";
            }

            if (message.Contains("unique"))
            {
                return "A record with this value already exists.";
            }

            if (message.Contains("not null"))
            {
                return "A required field is missing.";
            }
        }

        return "A database constraint was violated. Please check your input.";
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ValidationError>? Errors { get; set; }
}

public class ValidationError
{
    public string Property { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

