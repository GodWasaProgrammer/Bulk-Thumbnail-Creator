using BulkThumbnailCreator.Diagnostics;
using BulkThumbnailCreator.ServiceClasses;

namespace BulkThumbnailCreator.Interfaces;

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
