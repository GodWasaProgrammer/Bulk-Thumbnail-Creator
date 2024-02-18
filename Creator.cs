namespace BulkThumbnailCreator;

public partial class Creator
{
    public static async Task<List<PictureData>> Process(ProductionType ProdType, string url, List<string> texts, Settings settings, PictureData PicdataObjToVarietize = null)
    {
        settings.ListOfText = texts;

        #region Front Page Picture Line Up Output
        if (ProdType == ProductionType.FrontPagePictureLineUp)
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

            settings.TextAddedDir = settings.TextAddedDir + "/" + Path.GetFileNameWithoutExtension(settings.PathToVideo);

            Directory.CreateDirectory(settings.TextAddedDir);

            Settings.DownloadedVideosList.Add(settings.PathToVideo);

            // Adds To DownloadedVideosList if it is not already containing it,
            if (!Settings.DownloadedVideosList.Contains(settings.PathToVideo))
            {
                Settings.DownloadedVideosList.Add(settings.PathToVideo);
            }

            #region Run FfMpeg
            var parameters = new Dictionary<string, string>();

            string extractedfilename = Path.GetFileName(settings.PathToVideo);


            parameters["i"] = $@"""{extractedfilename}""";
            parameters["vf"] = "select='gt(scene,0.3)',select=key";
            parameters["vsync"] = "vfr";

            // get our current location
            string CurrentLoc = Assembly.GetExecutingAssembly().Location;

            string parentDirectory = Directory.GetParent(CurrentLoc).FullName;

            // if dir doesnt exist, make it
            string ExePath = Path.Combine(parentDirectory, "Executables");

            string truePath = Path.GetRelativePath(ExePath, settings.OutputDir);
            string pictureOutput = $@"""{truePath}/%03d.png""";

            FFmpegHandler fFmpegHandler = new();

            await FFmpegHandler.RunFFMPG(parameters, pictureOutput, settings);

            #endregion

            Settings.Memes = Directory.GetFiles(Settings.DankMemeStashDir, "*.*", SearchOption.AllDirectories);

            #region Face Detection

            settings.Files = Directory.GetFiles(settings.OutputDir, "*.*", SearchOption.AllDirectories);

            settings.LogService.LogInformation($"Processing {settings.Files.Length} images");

            // main loop for detecting faces, placing text where face is not
            for (int fileIndex = 0; fileIndex < settings.Files.Length; fileIndex++)
            {
                string file = settings.Files[fileIndex];

                Array2D<RgbPixel> image = null;

                try
                {
                    image = Dlib.LoadImage<RgbPixel>(file);
                    // Load image
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading image: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                }

                DlibDotNet.Rectangle[] FaceRectangles;

                using (var FACEDETECT = Dlib.GetFrontalFaceDetector())
                {
                    // Detect faces in the image
                    FaceRectangles = FACEDETECT.Operator(image);
                }
                #endregion

                #region Data Generation

                PictureData PassPictureData = new()
                {
                    FileName = file,
                    NumberOfBoxes = 2,
                };

                for (int AmountOfBoxes = 0; AmountOfBoxes < PassPictureData.NumberOfBoxes; AmountOfBoxes++)
                {
                    ParamForTextCreation currentParameters = new();

                    List<BoxType> PopulatedBoxes = [];

                    if (PassPictureData.BoxParameters.Count > 0)
                        PopulatedBoxes.Add(PassPictureData.BoxParameters[0].CurrentBox.Type);

                    currentParameters = DataGeneration.GetTextPosition(currentParameters, image, FaceRectangles, PopulatedBoxes, PassPictureData);

                    currentParameters = ColorData.DecideColorGeneration(currentParameters);

                    currentParameters.Font = DataGeneration.PickRandomFont();

                    // picks a random string from the list
                    Random pickAString = new();
                    int pickedString = pickAString.Next(settings.ListOfText.Count);
                    currentParameters.Text = settings.ListOfText[pickedString];

                    PassPictureData.BoxParameters.Add(currentParameters);
                }

                settings.PictureDatas.Add(PassPictureData);
            }
            #endregion

            #region Variety Data Generation
            //// Produce varietydata for the current object
            for (int i = 0; i < settings.PictureDatas.Count; i++)
            {
                Variety.Saturation(settings.PictureDatas[i]);

                Variety.Font(settings.PictureDatas[i]);

                Variety.PlacementOfText(settings.PictureDatas[i]);

                Variety.Random(settings.PictureDatas[i]);

                Variety.Meme(settings.PictureDatas[i]);
            }
            #endregion

            #region File Production
            //// File Production
            List<Task> productionTasks = [];

            foreach (PictureData picData in settings.PictureDatas)
            {
                bool NoBoxes = picData.BoxParameters.All(bp => bp.CurrentBox.Type == BoxType.None);

                if (!NoBoxes)
                {
                    Production production = new();
                    Task productionTask = Task.Run(() => Production.ProduceTextPictures(picData, settings));
                    productionTasks.Add(productionTask);
                }
            }

            await Task.WhenAll(productionTasks);
            #endregion

            #region Front Page Picture Line Up Mocking
            if (Mocking.BTCRunCount != 1)
            {
                // ffmpeg has finished, lets copy our mock data
                Mocking.OutPutDirMockCopy(settings);
            }
            #endregion
        }

        #endregion

        #region Variety Picture Output

        if (ProdType == ProductionType.VarietyList)
        {
            if (PicdataObjToVarietize == null)
            {
                settings.LogService.LogError("null has been passed to PicdataobjToVarietize");
            }
            else
            {
                List<Task> productionVarietyTaskList = [];

                foreach (PictureData picData in PicdataObjToVarietize.Varieties)
                {
                    Production production = new();
                    Task productionTask = Task.Run(() => Production.ProduceTextPictures(picData, settings));
                    productionVarietyTaskList.Add(productionTask);
                }

                await Task.WhenAll(productionVarietyTaskList);
            }

            if (Mocking.BTCRunCount != 1)
            {
                await Mocking.VarietiesList(settings);
            }
        }
        #endregion

        #region Custom Picture OutPut
        if (ProdType == ProductionType.CustomPicture)
        {
            if (PicdataObjToVarietize == null)
            {
                settings.LogService.LogError("Null has been passed to CustomPicture");
            }
            else
            {
                Production production = new();
                await Production.ProduceTextPictures(PicdataObjToVarietize, settings);
                settings.PictureDatas.Add(PicdataObjToVarietize);
            }
        }
        #endregion

        #region Make Showcase Video
        //Dictionary<string, string> paramToMakeVideoOfResult = new Dictionary<string, string>();
        //paramToMakeVideoOfResult["framerate"] = "2";
        //paramToMakeVideoOfResult["i"] = $@"""{Path.GetFullPath(BTCSettings.TextAddedDir)}/%03d.png""";
        //string getTruePath = Path.GetFullPath(BTCSettings.TextAddedDir);
        //string showCaseVideoOutPut = $@"""{getTruePath}/showcase.mp4""";

        //FFmpegHandler.RunFFMPG(paramToMakeVideoOfResult, showCaseVideoOutPut);
        #endregion

        #region Picdata serialization & Mock Setup

        if (Mocking.BTCRunCount != 1)
        {
            Mocking.SerializePicData(settings);
        }

        settings.Files = Directory.GetFiles(settings.OutputDir, "*.*", SearchOption.AllDirectories);

        settings.LogService.LogInformation("Processing Finished");

        if (ProdType == ProductionType.VarietyList)
        {
            Mocking.BTCRunCount++;
        }

        settings.JobService.CurrentJob.PictureDatas = settings.PictureDatas;
        settings.JobService.CurrentJob.Settings = settings;
        return settings.PictureDatas;
    }
    #endregion
    /// <summary>
    /// Mocking version of Process for debugging and testing
    /// </summary>
    /// <param name="prodtype"></param>
    /// <param name="url"></param>
    /// <param name="texts"></param>
    /// <param name="picdatatoMock"></param>
    /// <returns></returns>
    public static async Task<List<PictureData>> MockProcess(ProductionType prodtype, string url, List<string> texts, Settings settings, PictureData picdatatoMock = null)
    {
        settings.ListOfText = texts;

        if (prodtype == ProductionType.FrontPagePictureLineUp)
        {
            await Mocking.SetupFrontPagePictureLineUp(settings);
            settings.LogService.LogInformation("Mocking of FrontPagePictureLineUp complete");
        }

        ArgumentNullException.ThrowIfNull(texts);
        ArgumentNullException.ThrowIfNull(url);

        if (prodtype == ProductionType.VarietyList)
        {
            // fake variety list
            // this will never let you actually pick one, it will automatically mock to whatever was clicked
            // when the code ran last time
            if (picdatatoMock is null)
            {
                settings.LogService.LogError("null has been passed to PicdataobjToVarietize");
            }
            else
            {
                await Mocking.SetupVarietyDisplay(settings);
                settings.LogService.LogInformation("Mocking of Variety List complete");
            }
        }

        if (prodtype == ProductionType.CustomPicture)
        {

            if (picdatatoMock == null)
            {
                settings.LogService.LogError("Null has been passed to CustomPicture");
            }
            else
            {
                await Production.ProduceTextPictures(picdatatoMock, settings);
                settings.PictureDatas.Add(picdatatoMock);
            }
        }

        return settings.PictureDatas;
    }

    static void Main()
    {

    }

    [GeneratedRegex(@"[^\w\d]")]
    private static partial Regex CleanPathRegEx();
}