using Serilog;
using Serilog.Core;
using Services.Interfaces;
using System;

namespace Services
{
    public class LogService : ILogService
    {
        private readonly Logger _logger;

        private readonly object _lock = new object();

        public event Action<string> LogEntryAdded = delegate { };

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
            lock (_lock)
            {
                _logger.Information(message);
                LogEntryAdded?.Invoke(message);
            }
        }

        public void LogError(string message)
        {
            lock (_lock)
            {
                _logger.Error(message);
                LogEntryAdded?.Invoke(message);
            }
        }
        public void LogWarning(string message)
        {
            lock (_lock)
            {
                _logger.Warning(message);
                LogEntryAdded?.Invoke(message);
            }
        }

        public void LogException(string message)
        {
            lock (_lock)
            {
                _logger.Fatal(message);
                LogEntryAdded?.Invoke(message);
            }
        }

    }

}