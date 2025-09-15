using be_devextreme_starter.DTOs;
using System.Net;
using System.Text.Json;

namespace be_devextreme_starter.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
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
                var response = context.Response;
                response.ContentType = "application/json";

                // Log the exception
                _logger.LogError(ex, "An unhandled exception has occurred.");

                var apiResponse = ApiResponse<object>.Error(ex.Message);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var result = JsonSerializer.Serialize(apiResponse);
                await response.WriteAsync(result);
            }
        }
    }

}
