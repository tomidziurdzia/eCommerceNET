using System.Net;
using System.Text.Json;
using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.SharedLibrary.Middleware;

public class GlobalException(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Declare default variables
        string title = "Error";
        string message = "Sorry, internal server error occurred. Try again";
        int statusCode = (int)HttpStatusCode.InternalServerError;

        try
        {
            await next(context);

            // Check if Excpetion is Too Many Request // 429 status code

            if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
            {
                title = "Warning";
                message = "Too many request";
                statusCode = (int)StatusCodes.Status429TooManyRequests;
                await ModifyHeader(context, title, message, statusCode);
            }

            // If Response is UnAuthorized // 401 status code
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                title = "Alert";
                message = "You are not authorized to access";
                statusCode = (int)StatusCodes.Status401Unauthorized;
                await ModifyHeader(context, title, message, statusCode);
            }

            // If Response is Forbidden // 403 status code 
            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                title = "Out of Access";
                message = "You are not allowed";
                statusCode = (int)StatusCodes.Status403Forbidden;
                await ModifyHeader(context, title, message, statusCode);
            }
        }
        catch(Exception exception)
        {
            // Log Original Exceptions /File, Debugger, Console
            LogException.LogExceptions(exception);
            
            // Check if Exception is Timeout
            if (exception is TaskCanceledException || exception is TimeoutException)
            {
                title = "Out of time";
                message = "Request timeout, try again";
                statusCode = (int)StatusCodes.Status408RequestTimeout;
            }

            // If Exception is caught
            // If none of the exception || Exception caught then do the default
            await ModifyHeader(context, title, message, statusCode);
        }
    }

    private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
    {
        // Display scary-free message to client
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
        {
            Detail = message,
            Status = statusCode,
            Title = title
        }), CancellationToken.None);
        return;
    }
}