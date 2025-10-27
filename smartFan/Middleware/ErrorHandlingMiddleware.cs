using System.Net;
using System.Text.Json;
using smartFan.Services.Interfaces;

namespace smartFan.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); //proceed with request
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        
        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // Get the logger from request services
            var logger = context.RequestServices.GetRequiredService<ILoggerService>();
            
            //Log to my service
            logger.LogError($"Unhandled Exception in {context.Request.Path}: {ex.Message}", ex);

            //Response details
            context.Response.ContentType = "application/json";
            
            // Map exception types to appropriate HTTP status codes
            var (statusCode, message) = ex switch
            {
                ArgumentNullException => (HttpStatusCode.BadRequest, "Invalid input provided"),
                ArgumentException => (HttpStatusCode.BadRequest, "Invalid argument"),
                KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Access denied"),
                NotImplementedException => (HttpStatusCode.NotImplemented, "Feature not implemented"),
                TimeoutException => (HttpStatusCode.RequestTimeout, "Request timeout"),
                InvalidOperationException => (HttpStatusCode.Conflict, "Invalid operation"),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
            };

            //Send standardized error response
            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = message,
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}