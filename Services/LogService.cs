// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace BulkThumbnailCreator.Services
{
    public class LogService : ILogService
    {
        private readonly Serilog.Core.Logger _logger;

        public event Action<string> LogEntryAdded = delegate { };

        public LogService()
        {
            _logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/BulkThumbnailCreator.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        }

        public async Task LogInformation(string message)
        {
            await Task.Run(() =>
            {
                _logger.Information(message);
                LogEntryAdded?.Invoke(message);
            });
        }

        public async Task LogError(string message)
        {
            await Task.Run(() =>
            {
                _logger.Error(message);
                LogEntryAdded?.Invoke(message);
            });
        }

        public async Task LogWarning(string message)
        {
            await Task.Run(() =>
            {
                _logger.Warning(message);
                LogEntryAdded?.Invoke(message);
            });
        }

        public async Task LogException(string message)
        {
            await Task.Run(() =>
            {
                _logger.Fatal(message);
                LogEntryAdded?.Invoke(message);
            });
        }
    }
}