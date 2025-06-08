using BulkThumbnailCreator.Diagnostics;

namespace BulkThumbnailCreator.ServiceClasses;
public record JobExecutionReport
{
    public string JobId { get; init; }
    public string User { get; init; }
    public DateTime StartTime { get; init; }
    public string VideoUrl { get; init; }
    public Settings Settings { get; init; }
    public Dictionary<string, TimeSpan> Timings { get; init; }
    public PerformanceMetrics PerformanceMetrics { get; init; }
}
