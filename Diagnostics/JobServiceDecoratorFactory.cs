// 4. Skapa en fabriksklass för JobService
using BulkThumbnailCreator.Diagnostics;

public static class JobServiceDecoratorFactory
{
    public static IJobService Create(IJobService inner, IPerformanceTracker tracker)
    {
        return TimedServiceDecorator<IJobService>.Create(inner, tracker, "JobService");
    }
}
