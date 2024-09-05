namespace BulkThumbnailCreator.Services;

public class CreatorService
{
    public CreatorService(ILogService logger, JobService jobService, Creator creator)
    {
        // if we are not in a state where we have a job, we should clear the output directories
        // this should only be called when the app is started
        // or if the joblist has been cleared
        //if (UserStateService.UserJobs.Count == 0)
        //{
        //    ClearBaseOutPutDirectories();
        //}
        _logService = logger;
        _jobService = jobService;
        _creator = creator;
    }

    private ILogService _logService;

    private JobService _jobService;

    private Creator _creator;

    public static void ClearBaseOutPutDirectories(Settings settings)
    {
        DirectoryInfo di = new(settings.TextAddedDir);

        foreach (var file in di.GetFiles())
        {
            file.Delete();
        }
        foreach (var dir in di.GetDirectories())
        {
            dir.Delete(true);
        }

        DirectoryInfo di2 = new(settings.OutputDir);

        foreach (var file in di2.GetFiles())
        {
            file.Delete();
        }
        foreach (var dir in di2.GetDirectories())
        {
            dir.Delete(true);
        }
    }

    public event EventHandler<bool> LoadingStateChanged;

    private bool _isLoading = false;

    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                LoadingStateChanged?.Invoke(this, _isLoading);
            }
        }
    }

    public async Task CreateInitialPictureArrayAsync(string url, List<string> listOfTextToPrint, Job job)
    {
        // this is the initial building of the job object, after this the settings inside the job should not be modified
        // except by the job itself performing something
        IsLoading = true;
        job.State = States.Loading;
        job.Settings.ListOfText = listOfTextToPrint;
        job.TextToPrint = listOfTextToPrint;

        if (job.Settings.Mocking && job.Settings.MakeMocking)
        {
            job.PictureData = await _creator.MockProcess(ProductionType.FrontPagePictureLineUp, url, listOfTextToPrint, job);
        }
        else
        {
            job.VideoUrl = url;
            await _creator.FrontPageLineup(job);
        }

        if (job.Settings.PathToVideo != null)
        {
            job.VideoPath = job.Settings.PathToVideo;
            job.VideoName = Path.GetFileNameWithoutExtension(job.Settings.PathToVideo);
        }
        job.State = States.FrontPagePictureLineUp;
        IsLoading = false;
    }

    public async Task<List<string>> CreatePictureDataVariety(PictureData pictureData, Job job)
    {
        IsLoading = true;
        List<string> imageUrls = [];
        var url = string.Empty;

        if (job.Settings.Mocking && job.Settings.MakeMocking)
        {
            job.PictureData = await _creator.MockProcess(ProductionType.VarietyList, url, job.TextToPrint, job, pictureData);
            job.State = States.varietyList;
        }
        else
        {
            job.State = States.Loading;
            job.PictureData = await _creator.VarietyLineup(job, pictureData);
            job.State = States.varietyList;
        }

        var liftMockFolder = "";
        if (job.Settings.Mocking)
        {
            var subdirs = Directory.GetDirectories(job.Settings.TextAddedDir);
            foreach (var subdir in subdirs)
            {
                var lastFolderName = Path.GetFileName(subdir.TrimEnd(Path.DirectorySeparatorChar));
                liftMockFolder = lastFolderName;
            }
            liftMockFolder = liftMockFolder.Replace("varietyof", "");
            var path = Path.GetDirectoryName(job.PictureData[0].FileName);

            liftMockFolder = Path.Combine(path, liftMockFolder);
            var index = liftMockFolder.IndexOf("\\");
            liftMockFolder = liftMockFolder.Remove(index, 1).Insert(index, "/");

            pictureData = job.PictureData.Find(x => x.FileName == liftMockFolder);
        }

        var parentfilename = Path.GetFileName(pictureData.FileName);
        var concatenatedString = $"{job.Settings.TextAddedDir}/varietyof{parentfilename}";
        var arrayOfFilePaths = Directory.GetFiles(concatenatedString, "*.png");

        foreach (var filepath in arrayOfFilePaths)
        {
            var imageurl = $"/{filepath}"; // convert to URL
            imageUrls.Add(imageurl);
        }

        // the list of urls to be displayed in variety display
        job.VarietyUrls = imageUrls;
        IsLoading = false;

        return imageUrls;
    }

    public async Task<PictureData> CreateRandomVariety(PictureData pictureData, Job job)
    {
        IsLoading = true;

        var newData = await _creator.Random(job, pictureData);

        IsLoading = false;

        return newData;
    }

    public async Task<PictureData> CreateFontVariety(PictureData pictureData, Job job)
    {
        IsLoading = true;

        var newData = await _creator.FontVariety(job, pictureData);

        IsLoading = false;

        return newData;
    }
    public async Task<PictureData> CreateFXVariety(PictureData pictureData, Job job)
    {
        IsLoading = true;

        var newData = await _creator.SpecialEffectsVariety(job, pictureData);

        IsLoading = false;

        return newData;
    }

    public async Task<PictureData> CreateBoxVariety(PictureData pictureData, Job job)
    {
        IsLoading = true;

        var newData = await _creator.BoxVariety(job, pictureData);

        IsLoading = false;

        return newData;
    }

    public async Task<PictureData> CreateColorVariety(PictureData pictureData, Job job)
    {
        IsLoading = true;

        var newData = await _creator.ColorVariety(job, pictureData);

        IsLoading = false;

        return newData;
    }

    public async Task<PictureData> CreateCustomPicDataObject(PictureData pictureData, Job job)
    {
        _isLoading = true;
        job.State = States.CustomPicture;
        job.PictureData = await _creator.CustomPicture(job, pictureData);
        _isLoading = false;

        return pictureData;
    }

    public static Task<PictureData> SetPictureDataImageDisplayCorrelation(string imageUrl, Job job)
    {
        PictureData picData = new();

        if (job.Settings.Mocking && job.Settings.MakeMocking)
        {
            var dirToMockPicture = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "TextAdded");

            DirectoryInfo di = new(dirToMockPicture);

            var di2 = di.GetDirectories();

            var mockCorrelation = "";

            foreach (var directory in di2)
            {
                mockCorrelation = Path.GetFileName(directory.FullName);
            }

            foreach (var item in job.PictureData)
            {
                var numberOfPicture = Path.GetFileNameWithoutExtension(item.FileName);

                var numberOfPicture2 = Path.GetFileNameWithoutExtension(mockCorrelation);

                const string Varof = "varietyof ";

                numberOfPicture2 = numberOfPicture2.Remove(0, Varof.Length);

                if (numberOfPicture == numberOfPicture2)
                {
                    picData = new PictureData(item);
                    break;
                }
            }
        }
        else
        {
            foreach (var item in job.PictureData)
            {
                if (Path.GetFileNameWithoutExtension(item.OutPath) == Path.GetFileNameWithoutExtension(imageUrl))
                {
                    picData = new PictureData(item);
                    break;
                }
            }
        }

        return Task.FromResult(picData);
    }

    public static PictureData SetPictureDataImageDisplayCorrelationForVarietyList(string imageUrl, Job job)
    {
        PictureData picData = new();

        if (job.Settings.Mocking && job.Settings.MakeMocking)
        {
            // nonsense to just pick the first one
            var jobdata = job.PictureData[0];
            var jobvarietyData = jobdata.Varieties[0];
            picData = jobvarietyData;
        }
        else
        {
            foreach (var item in job.PictureData)
            {
                foreach (var variety in item.Varieties)
                {
                    if (Path.GetFileNameWithoutExtension(variety.OutPath) == Path.GetFileNameWithoutExtension(imageUrl))
                    {
                        picData = new PictureData(variety);
                        break;
                    }
                }
            }
        }
        return picData;
    }

    public async Task<string> FetchVideo(string urlToVideo, Settings settings)
    {
        var pathToVideo = await _creator.FetchVideo(urlToVideo, settings);
        return pathToVideo;
    }
}
