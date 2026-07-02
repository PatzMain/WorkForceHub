using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using EMS.Application.Common;

namespace EMS.Web.Middleware
{
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
                _logger.LogError(ex, "An unhandled exception has occurred during request processing.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var isApiRequest = context.Request.Path.StartsWithSegments("/api") || 
                               context.Request.Headers["Accept"].ToString().Contains("application/json");

            if (isApiRequest)
            {
                context.Response.ContentType = "application/json";
                var statusCode = StatusCodes.Status500InternalServerError;
                var message = "An internal server error occurred.";
                object? errors = null;

                if (exception is NotFoundException notFoundEx)
                {
                    statusCode = StatusCodes.Status404NotFound;
                    message = notFoundEx.Message;
                }
                else if (exception is ValidationException validationEx)
                {
                    statusCode = StatusCodes.Status400BadRequest;
                    message = validationEx.Message;
                    errors = validationEx.Errors;
                }

                context.Response.StatusCode = statusCode;
                
                var result = JsonSerializer.Serialize(new 
                { 
                    statusCode, 
                    message, 
                    errors 
                });
                
                await context.Response.WriteAsync(result);
            }
            else
            {
                // MVC View redirects
                if (exception is NotFoundException)
                {
                    context.Response.Redirect("/Home/Error404");
                }
                else
                {
                    context.Response.Redirect("/Home/Error");
                }
                await Task.CompletedTask;
            }
        }
    }
}
