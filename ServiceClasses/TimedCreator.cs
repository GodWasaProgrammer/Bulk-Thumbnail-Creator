using BulkThumbnailCreator.DataMethods;
using BulkThumbnailCreator.Wrappers;
using Microsoft.Extensions.Logging;

namespace BulkThumbnailCreator.Diagnostics;

public partial class TimedCreator : ICreator
{
    private readonly ICreator _inner;
    private readonly IPerformanceTracker _tracker;
    private readonly ILogger<TimedCreator> _logger;
    private readonly TimedProduction _timedProduction;

    public event EventHandler<bool> LoadingStateChanged
    {
        add => _inner.LoadingStateChanged += value;
        remove => _inner.LoadingStateChanged -= value;
    }

    public bool IsLoading
    {
        get => _inner.IsLoading;
        private set
        {
            var field = _inner.GetType().GetField("_isLoading", BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(_inner, value);
        }
    }

    public void ClearBaseOutPutDirectories()
    {
        using (_tracker.TrackOperation($"{nameof(Creator)}.{nameof(ClearBaseOutPutDirectories)}"))
        {
            _inner.ClearBaseOutPutDirectories();
        }
    }

    private async Task<(Array2D<RgbPixel>, Rectangle[])> FaceDetection(string file)
    {
        const string operationName = "FaceDetection";
        using (_tracker.TrackOperation($"Creator.{operationName}"))
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var task = (Task<(Array2D<RgbPixel>, Rectangle[])>)_faceDetectionMethod.Invoke(null, [file]);
                var result = await task;

                _logger.LogDebug($"{operationName} completed in {stopwatch.ElapsedMilliseconds}ms");
                return result;
            }
            catch (TargetInvocationException tex)
            {
                _logger.LogError(tex.InnerException ?? tex, $"{operationName} failed");
                throw new InvalidOperationException($"{operationName} execution error", tex.InnerException ?? tex);
            }
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
        using var overallOp = _tracker.TrackOperation($"{nameof(Creator)}.{nameof(FrontPageLineup_Thumbies)}");

        try
        {
            // Segment 1: Initiering
            using (var segment = _tracker.TrackOperation("1.InitialSetup"))
            {
                IsLoading = true;
                job.State = States.Loading;
                _timedProduction.CreateDirectories(job.Settings);
                await _timedProduction.VerifyDirectoryAndExeIntegrity(job.Settings);
            }

            // Segment 2: Videohämtning
            using (var segment = _tracker.TrackOperation("2.VideoDownload"))
            {
                await _timedProduction.YouTubeDL(job);
                CleanPathNames(job);
            }

            // Segment 3: Bilduttagning
            using (var segment = _tracker.TrackOperation("3.FrameExtraction"))
            {
                await RunFFMpeg(job.Settings);
                job.Settings.Memes = Directory.GetFiles(job.Settings.DankMemeStashDir, "*.*", SearchOption.AllDirectories);
                job.Settings.Files = Directory.GetFiles(job.Settings.OutputDir, "*.*", SearchOption.AllDirectories);
            }

            // Segment 4: Ansiktsdetektering
            using (var segment = _tracker.TrackOperation("4.FaceDetection"))
            {
                _logger.LogInformation($"Processing {job.Settings.Files.Length} images");
                foreach (var file in job.Settings.Files)
                {
                    var dataTuple = await FaceDetection(file);
                    var passPictureData = new PictureData { FileName = file, _numberOfBoxes = 2 };
                    CreateData(job, dataTuple.Item1, dataTuple.Item2, passPictureData);
                }
            }

            // Segment 5: Variationer
            using (var segment = _tracker.TrackOperation("5.VarietyGeneration"))
            {
                var dirWrapper = new DirectoryWrapper();
                var varietyInstance = new Variety(dirWrapper, job.Settings);
                for (var i = 0; i < job.PictureData.Count; i++)
                {
                    varietyInstance.Random(job.PictureData[i]);
                    varietyInstance.Meme(job.PictureData[i]);
                }
            }

            // Segment 6: Parallell produktion
            using (var segment = _tracker.TrackOperation("6.ParallelProduction"))
            {
                var semaphore = new SemaphoreSlim(4);
                var tasks = new List<Task>();

                foreach (var picData in job.PictureData)
                {
                    if (!picData.BoxParameters.All(bp => bp.CurrentBox.Type == BoxType.None))
                    {
                        await semaphore.WaitAsync();
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await _timedProduction.ProduceTextPictures(picData, job.Settings);
                                job.FrontLineUpUrls.Add(picData.OutPath);
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        }));
                    }
                }
                await Task.WhenAll(tasks);
            }

            // Segment 7: Avslut
            using (var segment = _tracker.TrackOperation("7.Finalization"))
            {
                if (Mocking.BTCRunCount != 1 && job.Settings.MakeMocking)
                {
                    Mocking.CopyOutPutDir(job.Settings);
                }
                job.State = States.FrontPagePictureLineUp;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in FrontPageLineup_Thumbies");
            throw;
        }
        finally
        {
            IsLoading = false;
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

    private async Task RunFFMpeg(Settings settings)
    {
        var method = typeof(Creator).GetMethod("RunFFMpeg", BindingFlags.NonPublic | BindingFlags.Instance);
        await (Task)method.Invoke(_inner, new object[] { settings });
    }

    private void CreateData(Job job, Array2D<RgbPixel> image, Rectangle[] faceRectangles, PictureData picData)
    {
        var method = typeof(Creator).GetMethod("CreateData", BindingFlags.NonPublic | BindingFlags.Instance);
        method.Invoke(_inner, new object[] { job, image, faceRectangles, picData });
    }

    private static readonly Regex _cleanPathRegex = new Regex(@"[^\w\d?]+", RegexOptions.Compiled);

    private void CleanPathNames(Job job)
    {
        job.Settings.OutputDir = Path.Combine(job.Settings.OutputDir,
            _cleanPathRegex.Replace(Path.GetFileNameWithoutExtension(job.Settings.PathToVideo), ""));
        Directory.CreateDirectory(job.Settings.OutputDir);

        job.Settings.TextAddedDir = Path.Combine(job.Settings.TextAddedDir,
            _cleanPathRegex.Replace(Path.GetFileNameWithoutExtension(job.Settings.PathToVideo), ""));
        Directory.CreateDirectory(job.Settings.TextAddedDir);
    }
    private static readonly MethodInfo _faceDetectionMethod;

    static TimedCreator()
    {
        _faceDetectionMethod = typeof(Creator)
            .GetMethod("FaceDetection",
                BindingFlags.NonPublic | BindingFlags.Static,
                null,
                [typeof(string)],
                null);

        if (_faceDetectionMethod == null)
        {
            throw new TypeInitializationException(typeof(TimedCreator).FullName,
                new MissingMethodException("FaceDetection method not found in Creator"));
        }
    }
    public TimedCreator(
        ICreator inner,
        IPerformanceTracker tracker,
        ILogger<TimedCreator> logger,
        ILogService logService)
    {
        _inner = inner;
        _tracker = tracker;
        _logger = logger;

        // Skapa en timed wrapper för den interna production-instansen
        var productionField = inner.GetType().GetField("_production", BindingFlags.NonPublic | BindingFlags.Instance);
        var originalProduction = (Production)productionField.GetValue(inner);
        _timedProduction = new TimedProduction(originalProduction, tracker);
    }
}
