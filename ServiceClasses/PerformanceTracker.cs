using System.Collections.Concurrent;
using BulkThumbnailCreator;
using Microsoft.Extensions.Logging;

public class PerformanceTracker : IPerformanceTracker
{
    private readonly ConcurrentQueue<OperationMetric> _recentOperations = new();
    private readonly Process _currentProcess = Process.GetCurrentProcess();
    private readonly Stopwatch _uptime = Stopwatch.StartNew();
    private readonly ILogger<PerformanceTracker> _logger;
    private const int MaxStoredOperations = 100;

    public PerformanceTracker(ILogger<PerformanceTracker> logger)
    {
        _logger = logger;
    }

    // Interface implementation
    public ITimedOperation TrackOperation(string operationName)
    {
        return new TimedOperation(operationName, this);
    }

    public PerformanceMetrics GetMetrics()
    {
        _currentProcess.Refresh();
        return new PerformanceMetrics
        {
            CpuUsagePercent = Math.Round(
                _currentProcess.TotalProcessorTime.TotalMilliseconds /
                _uptime.Elapsed.TotalMilliseconds * 100, 1),
            MemoryUsageMB = _currentProcess.WorkingSet64 / 1024 / 1024,
            TotalMemoryMB = GC.GetTotalMemory(false) / 1024 / 1024,
            ActiveThreads = _currentProcess.Threads.Count,
            ProcessCounters = new Dictionary<string, int>
            {
                ["Gen0Collections"] = GC.CollectionCount(0),
                ["Gen1Collections"] = GC.CollectionCount(1),
                ["Gen2Collections"] = GC.CollectionCount(2),
                ["Exceptions"] = ExceptionCounter.Count
            }
        };
    }

    public IEnumerable<OperationMetric> GetRecentOperations() => _recentOperations.ToArray();

    private class TimedOperation : ITimedOperation
    {
        private readonly string _operationName;
        private readonly PerformanceTracker _tracker;
        private readonly Stopwatch _stopwatch;

        public TimeSpan Elapsed => _stopwatch.Elapsed;

        public TimedOperation(string operationName, PerformanceTracker tracker)
        {
            _operationName = operationName;
            _tracker = tracker;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            var metric = new OperationMetric
            {
                Name = _operationName,
                Duration = _stopwatch.Elapsed,
                Timestamp = DateTime.Now,
                ThreadId = Environment.CurrentManagedThreadId
            };

            _tracker._recentOperations.Enqueue(metric);
            _tracker._logger.LogDebug(
                $"Operation '{_operationName}' completed in {_stopwatch.Elapsed.TotalMilliseconds}ms");

            while (_tracker._recentOperations.Count > MaxStoredOperations)
            {
                _tracker._recentOperations.TryDequeue(out _);
            }
        }
    }
}
