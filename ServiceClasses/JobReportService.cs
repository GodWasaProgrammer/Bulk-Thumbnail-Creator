using System.Collections.Concurrent;
using BulkThumbnailCreator;

public class JobReportService
{
    private readonly ConcurrentDictionary<string, JobExecutionReport> _reports = new();
    private readonly IPerformanceTracker _performanceTracker;

    public JobReportService(IPerformanceTracker performanceTracker)
    {
        _performanceTracker = performanceTracker;
    }

    public void AddReport(Job job, Dictionary<string, TimeSpan> segmentTimings)
    {
        var report = new JobExecutionReport
        {
            JobId = job.JobID.ToString(),
            User = job.User,
            StartTime = DateTime.Now,
            VideoUrl = job.VideoUrl,
            Settings = job.Settings,
            Timings = segmentTimings,
            PerformanceMetrics = _performanceTracker.GetMetrics() // Hämta CPU/minne etc.
        };

        _reports[job.JobID.ToString()] = report;
    }

    public List<JobExecutionReport> GetAllReports() => _reports.Values.ToList();
}

public record OperationMetric
{
    public string Name { get; init; }
    public TimeSpan Duration { get; init; }
    public DateTime Timestamp { get; init; }
    public int ThreadId { get; init; }
}

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

public static class ExceptionCounter
{
    private static int _count;
    public static int Count => _count;

    public static void Increment() => Interlocked.Increment(ref _count);
}
