using Microsoft.Extensions.Logging;

public class PerformanceTracker : IPerformanceTracker
{
    private readonly ILogger<PerformanceTracker> _logger;

    public PerformanceTracker(ILogger<PerformanceTracker> logger)
        => _logger = logger;

    public IDisposable TrackOperation(string operationName)
    {
        var sw = Stopwatch.StartNew();
        return new DisposableAction(() =>
        {
            sw.Stop();
            _logger.LogInformation("[PERF] {Operation} took {ElapsedMs}ms",
                operationName, sw.ElapsedMilliseconds);
        });
    }

    private class DisposableAction : IDisposable
    {
        private readonly Action _action;
        public DisposableAction(Action action) => _action = action;
        public void Dispose() => _action();
    }
}
