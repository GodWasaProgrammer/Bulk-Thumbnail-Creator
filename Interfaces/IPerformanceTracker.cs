using BulkThumbnailCreator;

public interface ITimedOperation : IDisposable
{
    TimeSpan Elapsed { get; }
}

public interface IPerformanceTracker
{
    ITimedOperation TrackOperation(string operationName);
    PerformanceMetrics GetMetrics();
    IEnumerable<OperationMetric> GetRecentOperations();
}
