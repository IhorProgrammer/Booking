
using BookingLibrary.JsonResponce;
using BookingLibrary.Services.LoggerService;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace BookingLibrary.ExceptionMiddleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;
        public ExceptionMiddleware(RequestDelegate next, ILoggerManager logger)
        {
            _logger = logger;
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex);
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(ResponseFormat.SERVER_ERROR.Responce)
            );
        }
    }
}
