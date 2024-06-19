namespace BulkThumbnailCreator;

public static partial class Creator
{
    public static async Task<string> FetchVideo(string url, Settings settings)
    {
        await Production.VerifyDirectoryAndExeIntegrity(settings);
        var PathToDownloadedVideo = await Production.YouTubeDL(url, settings);
        return PathToDownloadedVideo;
    }

    public static async Task<List<PictureData>> Process(ProductionType prodType, string url, List<string> texts, Settings settings, PictureData pictureData = null)
    {
        settings.ListOfText = texts;

        #region Front Page Picture Line Up Output
        if (prodType == ProductionType.FrontPagePictureLineUp)
        {
            settings.PictureDatas = [];

            // creates our 3 dirs to push out unedited thumbnails, and the edited thumbnails and also a path for where the downloaded youtube clips goes.
            Production.CreateDirectories(settings.OutputDir, settings.TextAddedDir, settings.YTDLOutPutDir, settings);

            await Production.VerifyDirectoryAndExeIntegrity(settings);

            settings.PathToVideo = await Production.YouTubeDL(url, settings);
            settings.OutputDir = "output";

            settings.OutputDir = settings.OutputDir + "/" + CleanPathRegEx().Replace(Path.GetFileNameWithoutExtension(settings.PathToVideo), "");
            Directory.CreateDirectory(settings.OutputDir);

            settings.TextAddedDir = "TextAdded";
            settings.TextAddedDir = settings.TextAddedDir + "/" + CleanPathRegEx().Replace(Path.GetFileNameWithoutExtension(settings.PathToVideo), "");
            Directory.CreateDirectory(settings.TextAddedDir);
            Settings.DownloadedVideosList.Add(settings.PathToVideo);

            // Adds To DownloadedVideosList if it is not already containing it,
            if (!Settings.DownloadedVideosList.Contains(settings.PathToVideo))
            {
                Settings.DownloadedVideosList.Add(settings.PathToVideo);
            }

            #region Run FfMpeg
            var parameters = new Dictionary<string, string>();

            var extractedfilename = Path.GetFileName(settings.PathToVideo);
            parameters["i"] = $@"""{extractedfilename}""";
            parameters["vf"] = "select='gt(scene,0.3)',select=key";
            parameters["vsync"] = "vfr";

            // get our current location
            var parentDirectory = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;

            var pictureOutput = $@"""{Path.GetRelativePath(Path.Combine(parentDirectory, "Executables"), settings.OutputDir)}/%03d.png""";

            await FFmpegHandler.RunFFMPG(parameters, pictureOutput, settings);

            #endregion

            Settings.Memes = Directory.GetFiles(Settings.DankMemeStashDir, "*.*", SearchOption.AllDirectories);

            #region Face Detection

            settings.Files = Directory.GetFiles(settings.OutputDir, "*.*", SearchOption.AllDirectories);

            await settings.LogService.LogInformation($"Processing {settings.Files.Length} images");
            Array2D<RgbPixel> image = null;
            Rectangle[] faceRectangles = null;

            for (var fileIndex = 0; fileIndex < settings.Files.Length; fileIndex++)
            {
                // Asynchronously load image and perform face detection in a non-blocking background thread
                await Task.Run(async () =>
                {
                    try
                    {
                        // Load image
                        image = await Task.Run(() => Dlib.LoadImage<RgbPixel>(settings.Files[fileIndex]));

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

                #endregion

                #region Data Generation

                PictureData passPictureData = new()
                {
                    FileName = settings.Files[fileIndex],
                    _numberOfBoxes = 2,
                };

                for (var amountOfBoxes = 0; amountOfBoxes < passPictureData._numberOfBoxes; amountOfBoxes++)
                {
                    ParamForTextCreation currentParameters = new();

                    List<BoxType> populatedBoxes = [];

                    if (passPictureData.BoxParameters.Count > 0)
                        populatedBoxes.Add(passPictureData.BoxParameters[0].CurrentBox.Type);

                    currentParameters = DataGeneration.GetTextPosition(currentParameters, image, faceRectangles, populatedBoxes);
                    currentParameters = ColorData.SelectTwoRandomColors(currentParameters);
                    currentParameters.Gradient = DataGeneration.RandomGradient();
                    currentParameters.Font = DataGeneration.PickRandomFont();

                    // picks a random string from the list
                    Random pickAString = new();
                    var pickedString = pickAString.Next(settings.ListOfText.Count);
                    currentParameters.Text = settings.ListOfText[pickedString];
                    passPictureData.BoxParameters.Add(currentParameters);
                }

                settings.PictureDatas.Add(passPictureData);
            }
            #endregion

            #region Variety Data Generation
            //// Produce varietydata for the current object
            for (var i = 0; i < settings.PictureDatas.Count; i++)
            {
                Variety.Random(settings.PictureDatas[i]);
                Variety.Meme(settings.PictureDatas[i]);
                ColorData.SelectedColors.Clear();
            }
            #endregion

            #region File Production

            SemaphoreSlim semaphore = new(4);
            List<Task> productionTasks = [];

            foreach (var picData in settings.PictureDatas)
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
                            await Production.ProduceTextPictures(picData, settings);
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

            #endregion

            #region Front Page Picture Line Up Mocking
            if (Mocking.BTCRunCount != 1 && Settings.MakeMocking)
            {
                // ffmpeg has finished, lets copy our mock data
                Mocking.CopyOutPutDir(settings);
            }
            #endregion
        }

        #endregion

        #region Variety Picture Output

        if (prodType == ProductionType.VarietyList)
        {
            if (pictureData == null)
            {
                await settings.LogService.LogError("null has been passed to PicdataobjToVarietize");
            }
            else
            {
                List<Task> productionVarietyTaskList = [];
                SemaphoreSlim semaphore = new(4);

                foreach (var picData in pictureData.Varieties)
                {
                    await semaphore.WaitAsync(); // Acquire a semaphore slot

                    var productionTask = Task.Run(async () =>
                    {
                        try
                        {
                            await Production.ProduceTextPictures(picData, settings);
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

            if (Mocking.BTCRunCount != 1 && Settings.MakeMocking)
            {
                await Mocking.CopyVarietyDir(settings);
            }
        }

        #endregion

        #region Custom Picture OutPut
        if (prodType == ProductionType.CustomPicture)
        {
            if (pictureData == null)
            {
                await settings.LogService.LogError("Null has been passed to CustomPicture");
            }
            else
            {
                await Production.ProduceTextPictures(pictureData, settings);
                settings.PictureDatas.Add(pictureData);
            }
        }
        #endregion

        #region Picdata serialization & Mock Setup

        if (Mocking.BTCRunCount != 1 && Settings.MakeMocking)
        {
            Mocking.SerializePicData(settings);
            Mocking.CopyTextAddedDir(settings);
        }

        settings.Files = Directory.GetFiles(settings.OutputDir, "*.*", SearchOption.AllDirectories);
        await settings.LogService.LogInformation("Processing Finished");

        if (prodType == ProductionType.VarietyList)
        {
            Mocking.BTCRunCount++;
        }

        return settings.PictureDatas;
    }
    #endregion
    /// <summary>
    /// Mocking version of Process for debugging and testing
    /// <param name="productionType"></param>
    /// <param name="url"></param>
    /// <param name="texts"></param>
    /// <param name="pictureData"></param>
    /// <returns></returns>
    public static async Task<List<PictureData>> MockProcess(ProductionType productionType, string url, List<string> texts, Settings settings, PictureData pictureData = null)
    {
        settings.ListOfText = texts;

        if (productionType == ProductionType.FrontPagePictureLineUp)
        {
            await Mocking.SetupFrontPagePictureLineUp(settings);
            await settings.LogService.LogInformation("Mocking of FrontPagePictureLineUp complete");
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
                await settings.LogService.LogError("null has been passed to PicdataobjToVarietize");
            }
            else
            {
                await Mocking.SetupVarietyDisplay(settings);
                await settings.LogService.LogInformation("Mocking of Variety List complete");
            }
        }

        if (productionType == ProductionType.CustomPicture)
        {
            if (pictureData == null)
            {
                await settings.LogService.LogError("Null has been passed to CustomPicture");
            }
            else
            {
                await Production.ProduceTextPictures(pictureData, settings);
                settings.PictureDatas.Add(pictureData);
            }
        }

        return settings.PictureDatas;
    }

    static void Main()
    {
        throw new NotSupportedException();
    }

    [GeneratedRegex(@"[^\w\d?]+")]
    private static partial Regex CleanPathRegEx();
}
