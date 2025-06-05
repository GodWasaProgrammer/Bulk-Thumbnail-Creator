namespace BulkThumbnailCreator;

public class PerformanceMetrics
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public double CpuUsagePercent { get; set; }
    public long MemoryUsageMB { get; set; }
    public long TotalMemoryMB { get; set; }
    public int ActiveThreads { get; set; }
    public Dictionary<string, int> ProcessCounters { get; set; } = new();
}
