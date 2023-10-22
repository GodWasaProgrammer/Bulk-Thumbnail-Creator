using Serilog;
using Serilog.Core;
using Serilog.Sinks.ListOfString;
using Bulk_Thumbnail_Creator.Interfaces;
using System.Collections.Generic;
using System;

namespace Bulk_Thumbnail_Creator.Services
{
    public class LogService : ILogService
    {
        private readonly Logger _logger;

        public event Action<string> LogEntryAdded;

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