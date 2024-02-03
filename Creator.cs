using BulkThumbnailCreator.Enums;
using BulkThumbnailCreator.PictureObjects;
using DlibDotNet;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BulkThumbnailCreator
{
    public class Creator
    {
        public static async Task<List<PictureData>> Process(ProductionType ProdType, string url, List<string> texts, Settings settings, PictureData PicdataObjToVarietize = null)
        {
            settings.ListOfText = texts;

            #region Front Page Picture Line Up Output
            if (ProdType == ProductionType.FrontPagePictureLineUp)
            {
                settings.PictureDatas = new();

                // creates our 3 dirs to push out unedited thumbnails, and the edited thumbnails and also a path for where the downloaded youtube clips goes.
                Production.CreateDirectories(settings.OutputDir, settings.TextAddedDir, settings.YTDLOutPutDir, settings);

                await Production.VerifyDirectoryAndExeIntegrity(settings);

                settings.PathToVideo = await Production.YouTubeDL(url, settings);
                settings.OutputDir = "output";

                settings.OutputDir = settings.OutputDir + "/" + Regex.Replace(Path.GetFileNameWithoutExtension(settings.PathToVideo), @"[^\w\d]", "");

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

                await fFmpegHandler.RunFFMPG(parameters, pictureOutput, settings);

                #endregion

                Settings.Memes = Directory.GetFiles(Settings.DankMemeStashDir, "*.*", SearchOption.AllDirectories);

                #region Face Detection
                //var faceDetector = new FaceDetector(0.3F, 0.4F, 0.5F);

                settings.Files = Directory.GetFiles(settings.OutputDir, "*.*", SearchOption.AllDirectories);

                settings.LogService.LogInformation($"Processing {settings.Files.Length} images");

                // main loop for detecting faces, placing text where face is not
                for (int fileIndex = 0; fileIndex < settings.Files.Length; fileIndex++)
                {
                    string file = settings.Files[fileIndex];

                    var image = Dlib.LoadImage<RgbPixel>(file);

                    DlibDotNet.Rectangle[] FaceRectangles;

                    using (var FACEDETECT = Dlib.GetFrontalFaceDetector())
                    {
                        // Detect faces in the image
                        FaceRectangles = FACEDETECT.Operator(image);
                    }

                    //Bitmap PicToDetectFacesOn = new(file);

                    //FaceDetectionResult[] faceDetectRes = faceDetector.Forward(PicToDetectFacesOn);

                    //System.Drawing.Rectangle[] facesRectArray = new System.Drawing.Rectangle[faceDetectRes.Length];

                    //// convert from FaceDetectionResult to Rectangle
                    //for (int i = 0; i < faceDetectRes.Length; i++)
                    //{
                    //    facesRectArray[i] = new System.Drawing.Rectangle
                    //    {
                    //        X = faceDetectRes[i].Rectangle.X,
                    //        Y = faceDetectRes[i].Rectangle.Y,

                    //        Width = faceDetectRes[i].Rectangle.Width,
                    //        Height = faceDetectRes[i].Rectangle.Height
                    //    };

                    //}
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

                        currentParameters = DataGeneration.GettextPosLinux(currentParameters, image, FaceRectangles, PassPictureData);

                        currentParameters = DataGeneration.DecideColorGeneration(currentParameters);

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
                    DataGeneration.GenSaturationVariety(settings.PictureDatas[i]);

                    DataGeneration.GenFontVariety(settings.PictureDatas[i]);

                    //DataGeneration.GenPlacementOfTextVariety(Settings.PictureDatas[i]);

                    DataGeneration.GenRandomVariety(settings.PictureDatas[i]);

                    DataGeneration.GenMemePosition(settings.PictureDatas[i]);
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
                        Task productionTask = Task.Run(() => production.ProduceTextPictures(picData, settings));
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
                    List<Task> productionVarietyTaskList = new();

                    foreach (PictureData picData in PicdataObjToVarietize.Varieties)
                    {
                        Production production = new();
                        Task productionTask = Task.Run(() => production.ProduceTextPictures(picData, settings));
                        productionVarietyTaskList.Add(productionTask);
                    }

                    await Task.WhenAll(productionVarietyTaskList);
                }

                //if (Mocking.BTCRunCount != 1)
                //{
                //    await Mocking.VarietiesList(settings);
                //}

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
                    await production.ProduceTextPictures(PicdataObjToVarietize, settings);
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

            //if (Mocking.BTCRunCount != 1)
            //{
            //    Mocking.SerializePicData(settings);
            //}

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
            // Settings.ListOfText = texts;

            if (prodtype == ProductionType.FrontPagePictureLineUp)
            {
                await Mocking.SetupFrontPagePictureLineUp(settings);
                //   Settings.LogService.LogInformation("Mocking of FrontPagePictureLineUp complete");
            }

            if (prodtype == ProductionType.VarietyList)
            {
                // fake variety list
                // this will never let you actually pick one, it will automatically mock to whatever was clicked
                // when the code ran last time
                if (picdatatoMock == null)
                {
                    //   Settings.LogService.LogError("null has been passed to PicdataobjToVarietize");
                }
                else
                {
                    await Mocking.SetupVarietyDisplay(settings);
                    //  Settings.LogService.LogInformation("Mocking of Variety List complete");
                }

            }

            if (prodtype == ProductionType.CustomPicture)
            {

                if (picdatatoMock == null)
                {
                    // Settings.LogService.LogError("Null has been passed to CustomPicture");
                }
                else
                {
                    Production Production = new();
                    //await Production.ProduceTextPictures(picdatatoMock,settings);
                    // Settings.PictureDatas.Add(picdatatoMock);
                }

            }

            return null; // Settings.PictureDatas;
        }

        static void Main()
        {

        }

    }

}