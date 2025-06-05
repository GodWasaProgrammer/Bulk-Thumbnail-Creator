using BulkThumbnailCreator;

public class TimedUserStateService : IUserStateService
{
    private readonly IUserStateService _inner;
    private readonly IPerformanceTracker _tracker;

    public TimedUserStateService(IUserStateService inner, IPerformanceTracker tracker)
    {
        _inner = inner;
        _tracker = tracker;
    }

    public List<Job> UserJobs
    {
        get => TrackPropertyGet(nameof(UserJobs), () => _inner.UserJobs);
        set => TrackPropertySet(nameof(UserJobs), () => _inner.UserJobs = value);
    }

    public void AddJob(Job job)
    {
        using (_tracker.TrackOperation($"{nameof(UserStateService)}.{nameof(AddJob)}"))
        {
            _inner.AddJob(job);
        }
    }

    public void RemoveJob(Job job)
    {
        using (_tracker.TrackOperation($"{nameof(UserStateService)}.{nameof(RemoveJob)}"))
        {
            _inner.RemoveJob(job);
        }
    }

    public Job GetJob(string user)
    {
        using (_tracker.TrackOperation($"{nameof(UserStateService)}.{nameof(GetJob)}"))
        {
            return _inner.GetJob(user);
        }
    }

    private T TrackPropertyGet<T>(string propertyName, Func<T> getter)
    {
        using (_tracker.TrackOperation($"{nameof(UserStateService)}.get_{propertyName}"))
        {
            return getter();
        }
    }

    private void TrackPropertySet(string propertyName, Action setter)
    {
        using (_tracker.TrackOperation($"{nameof(UserStateService)}.set_{propertyName}"))
        {
            setter();
        }
    }
}
