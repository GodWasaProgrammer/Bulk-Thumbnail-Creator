using BulkThumbnailCreator.DataMethods;
using BulkThumbnailCreator.Wrappers;

namespace BulkThumbnailCreator;

public partial class Creator
{
    public Creator(ILogService logger)
    {
        _logger = logger;
        _production = new Production(logger);

        // if we are not in a state where we have a job, we should clear the output directories
        // this should only be called when the app is started
        // or if the joblist has been cleared
        if (UserStateService.UserJobs.Count == 0)
        {
            ClearBaseOutPutDirectories();
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

    private readonly ILogService _logger;

    private readonly Production _production;

    public void ClearBaseOutPutDirectories()
    {
        var path = Environment.CurrentDirectory;
        path += "/TextAdded";
        DirectoryInfo di = new(path);

        foreach (var file in di.GetFiles())
        {
            file.Delete();
            _logger.LogInformation($"Deleted:{file.Name}");
        }
        foreach (var dir in di.GetDirectories())
        {
            dir.Delete(true);
            _logger.LogInformation($"Deleted:{dir.Name}");
        }

        path = Environment.CurrentDirectory;
        path += "/output";

        DirectoryInfo di2 = new(path);

        foreach (var file in di2.GetFiles())
        {
            file.Delete();
            _logger.LogInformation($"Deleted:{file.Name}");
        }
        foreach (var dir in di2.GetDirectories())
        {
            dir.Delete(true);
            _logger.LogInformation($"Deleted:{dir.Name}");
        }
    }

    private async Task<(Array2D<RgbPixel>, Rectangle[])> FaceDetection(string file)
    {
        Array2D<RgbPixel> image = null;
        Rectangle[] faceRectangles = null;

        // Asynchronously load image and perform face detection in a non-blocking background thread
        await Task.Run(async () =>
        {
            try
            {
                // Load image
                image = await Task.Run(() => Dlib.LoadImage<RgbPixel>(file));

                using var frontalFaceDetector = Dlib.GetFrontalFaceDetector();
                // Detect faces in the image
                faceRectangles = frontalFaceDetector.Operator(image);

                // Continue processing with image and faceRectangles...
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing image: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }

        });
        (Array2D<RgbPixel>, Rectangle[]) t1 = (image, faceRectangles);
        return t1;
    }

    private void CreateData(Job job, Array2D<RgbPixel> image, Rectangle[] faceRectangles, PictureData picData)
    {
        for (var amountOfBoxes = 0; amountOfBoxes < picData._numberOfBoxes; amountOfBoxes++)
        {
            ParamForTextCreation currentParameters = new();

            List<BoxType> populatedBoxes = [];

            if (picData.BoxParameters.Count > 0)
                populatedBoxes.Add(picData.BoxParameters[0].CurrentBox.Type);

            currentParameters.Boxes = DataGeneration.BuildDefaultBoxes(image);
            DataGeneration.GetTextPosition(currentParameters, faceRectangles, populatedBoxes);
            ColorData.SelectTwoRandomColors(currentParameters);
            currentParameters.Gradient = DataGeneration.RandomBool();
            currentParameters.Shadows = DataGeneration.RandomBool();

            var directoryWrapper = new DirectoryWrapper();
            var dg = new DataGeneration(directoryWrapper);
            currentParameters.Font = dg.PickRandomFont();

            // picks a random string from the list
            Random pickAString = new();
            var pickedString = pickAString.Next(job.TextToPrint.Count);
            currentParameters.Text = job.TextToPrint[pickedString];
            picData.BoxParameters.Add(currentParameters);
        }

        job.PictureData.Add(picData);
    }

    public async Task<string> FetchVideo(string url, Settings settings)
    {

        await _production.VerifyDirectoryAndExeIntegrity(settings);
        // var pathToDownloadedVideo = await Production.YouTubeDL(url, settings);
        return null;
    }
    private async Task RunFFMpeg(Settings settings)
    {
        var parameters = new Dictionary<string, string>();

        var extractedfilename = Path.GetFileName(settings.PathToVideo);
        parameters["i"] = $@"""{extractedfilename}""";
        parameters["vf"] = "select='gt(scene,0.3)',select=key";
        parameters["vsync"] = "vfr";

        // get our current location
        var parentDirectory = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;

        var pictureOutput = $@"""{Path.GetRelativePath(Path.Combine(parentDirectory, "Executables"), settings.OutputDir)}/%03d.png""";

        var ffmpg = new FFmpegHandler(_logger);
        await ffmpg.RunFFMPG(parameters, pictureOutput, settings);
    }

    public async Task FrontPageLineup(Job job)
    {
        IsLoading = true;
        job.State = States.Loading;
        // make ref for verbosity
        var settings = job.Settings;

        var prod = new Production(_logger);

        // creates our 3 dirs to push out unedited thumbnails, and the edited thumbnails and also a path for where the downloaded youtube clips goes.
        prod.CreateDirectories(settings);

        await prod.VerifyDirectoryAndExeIntegrity(settings);

        await prod.YouTubeDL(job);

        CleanPathNames(job);

        await RunFFMpeg(settings);

        job.Settings.Memes = Directory.GetFiles(job.Settings.DankMemeStashDir, "*.*", SearchOption.AllDirectories);

        settings.Files = Directory.GetFiles(settings.OutputDir, "*.*", SearchOption.AllDirectories);

        await _logger.LogInformation($"Processing {settings.Files.Length} images");
        foreach (var file in settings.Files)
        {
            var dataTuple = await FaceDetection(file);

            PictureData passPictureData = new()
            {
                FileName = file,
                _numberOfBoxes = 2,
            };
            CreateData(job, dataTuple.Item1, dataTuple.Item2, passPictureData);
        }

        //// Produce varietydata for the current object
        var dirWrapper = new DirectoryWrapper();
        var varietyInstance = new Variety(dirWrapper, job.Settings);
        for (var i = 0; i < job.PictureData.Count; i++)
        {
            varietyInstance.Random(job.PictureData[i]);
            varietyInstance.Meme(job.PictureData[i]);
        }

        SemaphoreSlim semaphore = new(4);
        List<Task> productionTasks = [];

        foreach (var picData in job.PictureData)
        {
            var noBoxes = picData.BoxParameters.All(bp => bp.CurrentBox.Type == BoxType.None);

            if (!noBoxes)
            {
                // Start the task asynchronously
                var productionTask = Task.Run(async () =>
                {
                    try
                    {
                        // Acquire a slot from the semaphore
                        await semaphore.WaitAsync();

                        // Execute the image processing task

                        await prod.ProduceTextPictures(picData, job.Settings);
                        job.FrontLineUpUrls.Add(picData.OutPath);
                    }
                    finally
                    {
                        // Release the slot when the task completes or throws an exception
                        semaphore.Release();
                    }
                });
                // Add the task to the list
                productionTasks.Add(productionTask);
            }
        }
        // Wait for all tasks to complete
        await Task.WhenAll(productionTasks);

        if (Mocking.BTCRunCount != 1 && job.Settings.MakeMocking)
        {
            // ffmpeg has finished, lets copy our mock data
            Mocking.CopyOutPutDir(job.Settings);
        }
        job.State = States.FrontPagePictureLineUp;
        IsLoading = false;
    }

    private void CleanPathNames(Job job)
    {
        job.Settings.OutputDir = job.Settings.OutputDir + "/" + CleanPathRegEx().Replace(Path.GetFileNameWithoutExtension(job.Settings.PathToVideo), "");
        Directory.CreateDirectory(job.Settings.OutputDir);

        job.Settings.TextAddedDir = job.Settings.TextAddedDir + "/" + CleanPathRegEx().Replace(Path.GetFileNameWithoutExtension(job.Settings.PathToVideo), "");
        Directory.CreateDirectory(job.Settings.TextAddedDir);
    }

    public async Task VarietyLineup(Job job, PictureData pictureData)
    {
        // set the delegates 
        IsLoading = true;
        job.State = States.Loading;

        if (pictureData == null)
        {
            await _logger.LogError("null has been passed to PicdataobjToVarietize");
        }
        else
        {
            List<Task> productionVarietyTaskList = [];
            SemaphoreSlim semaphore = new(4);
            job.VarietyUrls = new List<string>();
            foreach (var picData in pictureData.Varieties)
            {
                await semaphore.WaitAsync(); // Acquire a semaphore slot

                var productionTask = Task.Run(async () =>
                {
                    try
                    {
                        await _production.ProduceTextPictures(picData, job.Settings);
                        job.VarietyUrls.Add(picData.OutPath);
                    }
                    finally
                    {
                        semaphore.Release(); // Release the semaphore slot when done
                    }
                });

                productionVarietyTaskList.Add(productionTask);
            }
            await Task.WhenAll(productionVarietyTaskList);
        }

        if (Mocking.BTCRunCount != 1 && job.Settings.MakeMocking)
        {
            Mocking.SerializePicData(job.PictureData);
            await _logger.LogInformation("PicData has been Serialized");

            Mocking.CopyTextAddedDir(job.Settings);
            await _logger.LogInformation("TextAdded Dir has been copied for mocking");

            await Mocking.CopyVarietyDir(job.Settings);
            await _logger.LogInformation("Variety Dir has been copied");
        }

        Mocking.BTCRunCount++;
        job.Settings.Files = Directory.GetFiles(job.Settings.OutputDir, "*.*", SearchOption.AllDirectories);
        await _logger.LogInformation("Processing Finished");
        job.State = States.varietyList;
        IsLoading = false;
    }

    public async Task<PictureData> CustomPicture(Job job, PictureData pictureData)
    {
        if (pictureData == null)
        {
            await _logger.LogError("Null has been passed to CustomPicture");
        }
        else
        {
            await _production.ProduceTextPictures(pictureData, job.Settings);
            job.PictureData.Add(pictureData);
        }
        return pictureData;
    }

    public async Task Random(Job job, PictureData pictureData)
    {
        IsLoading = true;
        job.VarietyUrls.Clear();
        var dirWrapper = new DirectoryWrapper();
        var varietyClass = new Variety(dirWrapper, job.Settings);
        if (job.State == States.varietyList)
        {
            // guard against reproducing old objects
            pictureData.Varieties.Clear();
        }
        varietyClass.Random(pictureData);

        foreach (var variety in pictureData.Varieties)
        {
            await _production.ProduceTextPictures(variety, job.Settings);
            job.VarietyUrls.Add(variety.OutPath);
        }
        IsLoading = false;
    }

    public async Task FontVariety(Job job, PictureData pictureData)
    {
        IsLoading = true;
        pictureData.Varieties.Clear();
        DirectoryWrapper directoryWrapper = new();
        Variety variety = new(directoryWrapper, job.Settings);
        await variety.Fonts(pictureData);

        // we will clear the state for the frontend for easy lookups
        job.VarietyUrls.Clear();

        foreach (var varietyData in pictureData.Varieties)
        {
            if (varietyData.OutPutType is OutputType.FontVariety)
            {
                await _production.ProduceTextPictures(varietyData, job.Settings);
                job.VarietyUrls.Add(varietyData.OutPath);
            }
        }
        IsLoading = false;
    }

    public async Task BoxVariety(Job job, PictureData pictureData)
    {
        IsLoading = true;
        pictureData.Varieties.Clear();
        Variety.Boxes(pictureData);
        job.VarietyUrls.Clear();

        foreach (var varietyData in pictureData.Varieties)
        {
            await _production.ProduceTextPictures(varietyData, job.Settings);
            job.VarietyUrls.Add(varietyData.OutPath);
        }
        IsLoading = false;
    }

    public async Task SpecialEffectsVariety(Job job, PictureData pictureData)
    {
        IsLoading = true;
        pictureData.Varieties.Clear();
        Variety.FX(pictureData);
        job.VarietyUrls.Clear();

        foreach (var varietyData in pictureData.Varieties)
        {
            await _production.ProduceTextPictures(varietyData, job.Settings);
            job.VarietyUrls.Add(varietyData.OutPath);
        }
        IsLoading = false;
    }

    public async Task ColorVariety(Job job, PictureData pictureData)
    {
        IsLoading = true;
        pictureData.Varieties.Clear();
        Variety.Colors(pictureData);
        job.VarietyUrls.Clear();

        foreach (var varietyData in pictureData.Varieties)
        {
            await _production.ProduceTextPictures(varietyData, job.Settings);
            job.VarietyUrls.Add(varietyData.OutPath);
        }
        IsLoading = false;
    }

    public static PictureData FindPictureDataByImageUrl(string imageUrl, Job job)
    {
        PictureData foundPictureData = null;

        // Mocking-läge
        if (job.Settings.Mocking && job.Settings.MakeMocking)
        {
            var dirToMockPicture = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "TextAdded");
            DirectoryInfo di = new(dirToMockPicture);

            var directories = di.GetDirectories();
            string mockCorrelation = directories.FirstOrDefault()?.FullName;

            if (mockCorrelation != null)
            {
                string mockPictureName = Path.GetFileNameWithoutExtension(mockCorrelation);

                const string VarOfPrefix = "varietyof ";
                if (mockPictureName.StartsWith(VarOfPrefix))
                {
                    mockPictureName = mockPictureName.Substring(VarOfPrefix.Length);
                }

                foundPictureData = FindPictureDataByName(mockPictureName, job.PictureData);
            }
        }
        else
        {
            foundPictureData = FindPictureDataRecursively(imageUrl, job.PictureData);
        }

        return foundPictureData ?? new PictureData();
    }

    private static PictureData FindPictureDataRecursively(string imageUrl, IEnumerable<PictureData> pictureDataList)
    {
        foreach (var pictureData in pictureDataList)
        {
            if (Path.GetFileNameWithoutExtension(pictureData.OutPath) == Path.GetFileNameWithoutExtension(imageUrl))
            {
                return pictureData;
            }

            if (pictureData.Varieties.Count is not 0)
            {
                var foundInVarieties = FindPictureDataRecursively(imageUrl, pictureData.Varieties);
                if (foundInVarieties != null)
                {
                    return foundInVarieties;
                }
            }
        }
        return null;
    }

    // Hitta PictureData baserat på namn (används för mocking)
    private static PictureData FindPictureDataByName(string pictureName, IEnumerable<PictureData> pictureDataList)
    {
        foreach (var pictureData in pictureDataList)
        {
            var numberOfPicture = Path.GetFileNameWithoutExtension(pictureData.FileName);
            if (numberOfPicture == pictureName)
            {
                return new PictureData(pictureData);
            }
        }
        return null;
    }


    /// <summary>
    /// Mocking version of Process for debugging and testing
    /// <param name="productionType"></param>
    /// <param name="url"></param>
    /// <param name="texts"></param>
    /// <param name="pictureData"></param>
    /// <returns></returns>
    public async Task<List<PictureData>> MockProcess(ProductionType productionType, string url, List<string> texts, Job job, PictureData pictureData = null)
    {
        job.TextToPrint = texts;

        if (productionType == ProductionType.FrontPagePictureLineUp)
        {
            await Mocking.SetupFrontPagePictureLineUp(job);
            await _logger.LogInformation("Mocking of FrontPagePictureLineUp complete");
        }

        ArgumentNullException.ThrowIfNull(texts);
        ArgumentNullException.ThrowIfNull(url);

        if (productionType == ProductionType.VarietyList)
        {
            // fake variety list
            // this will never let you actually pick one, it will automatically mock to whatever was clicked
            // when the code ran last time
            if (pictureData is null)
            {
                await _logger.LogError("null has been passed to PicdataobjToVarietize");
            }
            else
            {
                await Mocking.SetupVarietyDisplay(job.Settings);
                await _logger.LogInformation("Mocking of Variety List complete");
            }
        }

        if (productionType == ProductionType.CustomPicture)
        {
            if (pictureData == null)
            {
                await _logger.LogError("Null has been passed to CustomPicture");
            }
            else
            {
                await _production.ProduceTextPictures(pictureData, job.Settings);
                job.PictureData.Add(pictureData);
            }
        }

        return job.PictureData;
    }

    static void Main()
    {
        throw new NotSupportedException();
    }

    [GeneratedRegex(@"[^\w\d?]+")]
    private static partial Regex CleanPathRegEx();
}
