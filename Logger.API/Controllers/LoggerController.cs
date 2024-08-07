using Logger.API.Models;
using Logger.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logger.API.Controllers
{
    [Route("/")]
    [ApiController]
    public class LoggerController : ControllerBase
    {
        private readonly ILogger<LoggerController> _logger;

        public LoggerController(ILogger<LoggerController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation(new LogException(new LogModel { Level = LogLevel.Information, Message = "Logger is working", Stacktrace = "" }), "Logger is working");
            return Ok("Hello! Logger is working");
        }

        [HttpPost]
        public String Post([FromBody] LogModel logModel)
        {
            switch (logModel.Level)
            {
                case LogLevel.Trace:
                    _logger.LogTrace(new LogException(logModel), logModel.Message);
                    break;
                case LogLevel.Debug:
                    _logger.LogDebug(new LogException(logModel), logModel.Message);
                    break;
                case LogLevel.Information:
                    _logger.LogInformation(new LogException(logModel), logModel.Message);
                    break;
                case LogLevel.Warning:
                    _logger.LogWarning(new LogException(logModel), logModel.Message);
                    break;
                case LogLevel.Error:
                    _logger.LogError(new LogException(logModel), logModel.Message);
                    break;
                case LogLevel.Critical:
                    _logger.LogCritical(new LogException(logModel), logModel.Message);
                    break;
                default:
                    _logger.LogInformation(new LogException(logModel), logModel.Message);
                    break;
            }

            return JsonResponceFormat<String>.GetResponce("success", 200, "log created", ""); ;
        }

    }
}
