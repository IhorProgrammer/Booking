
using BookingLibrary.JsonResponce;
using BookingLibrary.Services.LoggerService;
using BookingLibrary.Services.MessageSender;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace BookingLibrary.ExceptionMiddleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;
        private readonly IMessageSender _message;

        public ExceptionMiddleware(RequestDelegate next, ILoggerManager logger, IMessageSender message)
        {
            _logger = logger;
            _next = next;
            _message = message;
        }


        private void LogMessage(JsonResponseFormatException jsfe, LogLevel level)
        {
            switch (level)
            {
                case Microsoft.Extensions.Logging.LogLevel.Warning: _logger.LogWarning(jsfe); break;
                case Microsoft.Extensions.Logging.LogLevel.Information: _logger.LogInformation(jsfe); break;
                case Microsoft.Extensions.Logging.LogLevel.Debug: _logger.LogDebug(jsfe); break;
                case Microsoft.Extensions.Logging.LogLevel.Error: _logger.LogError(jsfe); break;
                case Microsoft.Extensions.Logging.LogLevel.Critical: _logger.LogError(jsfe); break;
            }
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch(JsonResponseFormatException jsfe)
            {
                if (jsfe.JsonResponse.email != null) 
                {
                    _message.Send( jsfe.JsonResponse.email.JWT, jsfe.JsonResponse.email.EmailType);
                    LogMessage(jsfe, jsfe.JsonResponse.email.EmailType.LogLevel);
                }

                //Відправка вкладеного повідомлення з певною критичністю
                if ( jsfe.InnerException != null ) LogMessage(jsfe, jsfe.LogLevel);
                
                await HandleResponseAsync(httpContext, jsfe.JsonResponse);
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
        private async Task HandleResponseAsync(HttpContext context, ResponseFormatData jrf)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)jrf.Code;

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(jrf.Responce)
            );
        }

    }
}
