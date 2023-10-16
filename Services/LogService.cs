using Serilog;
using Serilog.Core;
using Bulk_Thumbnail_Creator.Interfaces;

namespace Bulk_Thumbnail_Creator.Services
{
    public class LogService : ILogService
    {
        private readonly Logger _logger;

        //private readonly ILogService _log;

        public LogService()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("Bulk_Thumbnail_Creator.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            //_log = log;
        }

        public void LogInformation(string message)
        {
            _logger.Information(message);
        }

        public void LogError(string message)
        {
            _logger.Error(message);
        }
        public void LogWarning(string message)
        {
            _logger.Warning(message);
        }

        public void LogException(string message) 
        {
            _logger.Fatal(message);
        }
    }

}