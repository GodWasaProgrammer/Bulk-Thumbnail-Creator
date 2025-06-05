namespace BulkThumbnailCreator;

public class TimedServiceDecorator<TService> : DispatchProxy where TService : class
{
    private TService? _inner;
    private IPerformanceTracker? _tracker;
    private string? _serviceName;

    // 2. Lägg till denna metod istället för en konstruktor
    public void Initialize(TService inner, IPerformanceTracker tracker, string? serviceName = null)
    {
        _inner = inner;
        _tracker = tracker;
        _serviceName = serviceName ?? typeof(TService).Name;
    }

    protected override object? Invoke(MethodInfo? method, object?[]? args)
    {
        if (_inner == null || _tracker == null || method == null)
            throw new InvalidOperationException("Decorator not initialized");

        using (_tracker.TrackOperation($"{_serviceName}.{method.Name}"))
        {
            return method.Invoke(_inner, args);
        }
    }

    // 3. Statisk fabriksmetod
    public static TService Create(TService inner, IPerformanceTracker tracker, string? serviceName = null)
    {
        object proxy = Create<TService, TimedServiceDecorator<TService>>();
        ((TimedServiceDecorator<TService>)proxy).Initialize(inner, tracker, serviceName);
        return (TService)proxy;
    }
}
