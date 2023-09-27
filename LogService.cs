using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole;
using Serilog.Sinks.File;
using System.IO;

namespace Bulk_Thumbnail_Creator
{
    public interface ILoggerService
    {
        void LogInformation(string message);
        void LogError(string message);
        // Add more logging methods as needed
    }

    public class LogService : ILoggerService
    {
        private readonly ILogger _logger;

        public LogService()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("Bulk_Thumbnail_Creator.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public void LogInformation(string message)
        {
            _logger.Information(message);
        }

        public void LogError(string message)
        {
            _logger.Error(message);
        }

        // Implement other logging methods as needed
    }

}

