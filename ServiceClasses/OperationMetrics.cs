namespace BulkThumbnailCreator.ServiceClasses;

public record OperationMetric
{
    public string Name { get; init; }
    public TimeSpan Duration { get; init; }
    public DateTime Timestamp { get; init; }
    public int ThreadId { get; init; }
}
