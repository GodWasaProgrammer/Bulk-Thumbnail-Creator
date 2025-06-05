using Microsoft.Extensions.Logging;

public interface IPerformanceTracker
{
    IDisposable TrackOperation(string operationName);
}
