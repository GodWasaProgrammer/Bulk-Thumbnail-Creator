using BulkThumbnailCreator.Interfaces;
using Serilog;
using Serilog.Core;

namespace BulkThumbnailCreator.Services
{
    public class LogService : ILogService
    {
        private readonly Logger _logger;

        public event Action<string> LogEntryAdded = delegate { };

        public LogService()
        {
            _logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/BulkThumbnailCreator.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        }

        public void LogInformation(string message)
        {
            _logger.Information(message);
            LogEntryAdded?.Invoke(message);
        }

        public void LogError(string message)
        {
            _logger.Error(message);
            LogEntryAdded?.Invoke(message);
        }
        public void LogWarning(string message)
        {
            _logger.Warning(message);
            LogEntryAdded?.Invoke(message);
        }

        public void LogException(string message)
        {
            _logger.Fatal(message);
            LogEntryAdded?.Invoke(message);
        }
    }
}