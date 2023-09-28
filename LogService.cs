using Serilog;

namespace Bulk_Thumbnail_Creator
{
    public interface ILoggerService
    {
        void LogInformation(string message);
        void LogError(string message);

    }

    public class LogService : ILoggerService
    {
        private readonly Serilog.Core.Logger _logger;

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

    }

}

