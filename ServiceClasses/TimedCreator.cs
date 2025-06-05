using Microsoft.Extensions.Logging;

namespace BulkThumbnailCreator.Diagnostics;

public class TimedCreator : ICreator
{
    private readonly ICreator _inner;
    private readonly IPerformanceTracker _tracker;
    private readonly ILogger<TimedCreator> _logger;

    public TimedCreator(ICreator inner, IPerformanceTracker tracker, ILogger<TimedCreator> logger)
    {
        _inner = inner;
        _tracker = tracker;
        _logger = logger;
    }

    public event EventHandler<bool> LoadingStateChanged
    {
        add => _inner.LoadingStateChanged += value;
        remove => _inner.LoadingStateChanged -= value;
    }

    public bool IsLoading => _inner.IsLoading;

    public void ClearBaseOutPutDirectories()
    {
        using (_tracker.TrackOperation($"{nameof(Creator)}.{nameof(ClearBaseOutPutDirectories)}"))
        {
            _inner.ClearBaseOutPutDirectories();
        }
    }

    public async Task<string> FetchVideo(string ytlink, Settings settings)
    {
        using (_tracker.TrackOperation($"{nameof(Creator)}.{nameof(FetchVideo)}"))
        {
            return await _inner.FetchVideo(ytlink, settings);
        }
    }

    public async Task FrontPageLineup_Thumbies(Job job)
    {
        using (_tracker.TrackOperation($"{nameof(Creator)}.{nameof(FrontPageLineup_Thumbies)}"))
        {
            await _inner.FrontPageLineup_Thumbies(job);
        }
    }

    public async Task FrontPageLineup(Job job)
    {
        using (_tracker.TrackOperation($"{nameof(Creator)}.{nameof(FrontPageLineup)}"))
        {
            await _inner.FrontPageLineup(job);
        }
    }

    public async Task VarietyLineup(Job job, PictureData pictureData)
    {
        using (_tracker.TrackOperation($"{nameof(Creator)}.{nameof(VarietyLineup)}"))
        {
            await _inner.VarietyLineup(job, pictureData);
        }
    }

    public async Task<PictureData> CustomPicture(Job job, PictureData pictureData)
    {
        using (_tracker.TrackOperation($"{nameof(Creator)}.{nameof(CustomPicture)}"))
        {
            return await _inner.CustomPicture(job, pictureData);
        }
    }

    public async Task Random(Job job, PictureData pictureData)
    {
        using (_tracker.TrackOperation($"{nameof(Creator)}.{nameof(Random)}"))
        {
            await _inner.Random(job, pictureData);
        }
    }

    public async Task FontVariety(Job job, PictureData pictureData)
    {
        using (_tracker.TrackOperation($"{nameof(Creator)}.{nameof(FontVariety)}"))
        {
            await _inner.FontVariety(job, pictureData);
        }
    }

    public async Task BoxVariety(Job job, PictureData pictureData)
    {
        using (_tracker.TrackOperation($"{nameof(Creator)}.{nameof(BoxVariety)}"))
        {
            await _inner.BoxVariety(job, pictureData);
        }
    }

    public async Task SpecialEffectsVariety(Job job, PictureData pictureData)
    {
        using (_tracker.TrackOperation($"{nameof(Creator)}.{nameof(SpecialEffectsVariety)}"))
        {
            await _inner.SpecialEffectsVariety(job, pictureData);
        }
    }

    public async Task ColorVariety(Job job, PictureData pictureData)
    {
        using (_tracker.TrackOperation($"{nameof(Creator)}.{nameof(ColorVariety)}"))
        {
            await _inner.ColorVariety(job, pictureData);
        }
    }

    public async Task<List<PictureData>> MockProcess(ProductionType productionType, string url, List<string> texts, Job job, PictureData pictureData = null)
    {
        using (_tracker.TrackOperation($"{nameof(Creator)}.{nameof(MockProcess)}"))
        {
            return await _inner.MockProcess(productionType, url, texts, job, pictureData);
        }
    }

    // Hjälpmetoder för att hantera statiska anrop
    private static PictureData FindPictureDataByImageUrl(string imageUrl, Job job)
    {
        return Creator.FindPictureDataByImageUrl(imageUrl, job);
    }

    private static PictureData FindPictureDataRecursively(string imageUrl, IEnumerable<PictureData> pictureDataList)
    {
        return Creator.FindPictureDataRecursively(imageUrl, pictureDataList);
    }

    private static PictureData FindPictureDataByName(string pictureName, IEnumerable<PictureData> pictureDataList)
    {
        return Creator.FindPictureDataByName(pictureName, pictureDataList);
    }
}
