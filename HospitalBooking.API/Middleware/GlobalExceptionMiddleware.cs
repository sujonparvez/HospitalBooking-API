using System.Net;
using System.Text.Json;
using FluentValidation;
using HospitalBooking.Application.Common.Models;
using Microsoft.AspNetCore.Http; // For HttpContext
// using FluentValidation; // If using FluentValidation, catch ValidationException

namespace HospitalBooking.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = new ApiResponse<string>() { Success = false };

        switch (exception)
        {
            case KeyNotFoundException e:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = e.Message;
                break;
            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Message = "Unauthorized";
                break;
            case ArgumentException e:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = e.Message;
                break;
            case InvalidOperationException e:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = e.Message;
                break;
            case ValidationException e:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = e.Message;
                break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "Internal Server Error";
                // In Dev, maybe show stack trace? For now, keep generic.
                break;
        }

        response.Errors = new List<string> { exception.Message };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, jsonOptions);

        return context.Response.WriteAsync(json);
    }
}
