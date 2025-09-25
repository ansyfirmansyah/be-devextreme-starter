using be_devextreme_starter.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace be_devextreme_starter.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred. TraceId: {TraceId}", context.TraceIdentifier);

                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // Kirim ex.Message. Sembunyikan jika di production jika perlu.
                var errorMessage = "An unexpected error occurred. Please contact support and provide the Trace ID.";

                // Jika Anda ingin menampilkan detail error di environment development:
                if (_env.IsDevelopment())
                {
                    errorMessage = ex.Message;
                }

                var apiResponse = ApiResponse<object>.Error(errorMessage, context.Response.StatusCode, context.TraceIdentifier);

                var result = JsonSerializer.Serialize(apiResponse);
                await context.Response.WriteAsync(result);
            }
        }
    }

}
