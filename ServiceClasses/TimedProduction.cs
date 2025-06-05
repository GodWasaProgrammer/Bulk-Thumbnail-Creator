using BulkThumbnailCreator;

public class TimedProduction : IProduction
{
    private readonly IProduction _inner;
    private readonly IPerformanceTracker _tracker;

    public TimedProduction(IProduction inner, IPerformanceTracker tracker)
    {
        _inner = inner;
        _tracker = tracker;
    }

    public void CreateDirectories(Settings settings)
    {
        using (_tracker.TrackOperation($"{nameof(Production)}.{nameof(CreateDirectories)}"))
        {
            _inner.CreateDirectories(settings);
        }
    }

    public async Task VerifyDirectoryAndExeIntegrity(Settings settings)
    {
        using (_tracker.TrackOperation($"{nameof(Production)}.{nameof(VerifyDirectoryAndExeIntegrity)}"))
        {
            await _inner.VerifyDirectoryAndExeIntegrity(settings);
        }
    }

    public async Task YouTubeDL(Job job)
    {
        using (_tracker.TrackOperation($"{nameof(Production)}.{nameof(YouTubeDL)}"))
        {
            await _inner.YouTubeDL(job);
        }
    }

    public async Task ProduceTextPictures(PictureData picData, Settings settings)
    {
        using (_tracker.TrackOperation($"{nameof(Production)}.{nameof(ProduceTextPictures)}"))
        {
            await _inner.ProduceTextPictures(picData, settings);
        }
    }
}
