using System.Net;
using System.Text.Json;

namespace StudyPlanner.Application.Middleware
{
    public class GlobalErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlerMiddleware> _logger;

        public GlobalErrorHandlerMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var statusCode = exception switch
            {
                ArgumentException => (int)HttpStatusCode.BadRequest,  
                KeyNotFoundException => (int)HttpStatusCode.NotFound, 
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized, 
                _ => (int)HttpStatusCode.InternalServerError  
            };

            response.StatusCode = statusCode;

            var errorResponse = new
            {
                StatusCode = response.StatusCode,
                Message = exception.Message,
                Detailed = exception.InnerException?.Message
            };

            var json = JsonSerializer.Serialize(errorResponse);
            return response.WriteAsync(json);
        }
    }

}
