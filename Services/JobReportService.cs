using System.Collections.Concurrent;
using BulkThumbnailCreator.ServiceClasses;

namespace BulkThumbnailCreator.Services;

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
