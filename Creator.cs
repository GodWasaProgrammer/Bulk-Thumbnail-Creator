using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.PictureObjects;
using FaceONNX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bulk_Thumbnail_Creator
{
    public class Creator
    {
        /// <summary>
        /// Mocking version of Process for debugging and testing
        /// </summary>
        /// <param name="prodtype"></param>
        /// <param name="url"></param>
        /// <param name="texts"></param>
        /// <param name="picdatatoMock"></param>
        /// <returns></returns>

        public static async Task<List<PictureData>> Process(ProductionType ProdType, string url, List<string> texts, PictureData PicdataObjToVarietize = null)
        {
            Settings.ListOfText = texts;

            #region Front Page Picture Line Up Output
            if (ProdType == ProductionType.FrontPagePictureLineUp)
            {
                // creates our 3 dirs to push out unedited thumbnails, and the edited thumbnails and also a path for where the downloaded youtube clips goes.
                Production.CreateDirectories(Settings.OutputDir, Settings.TextAddedDir, Settings.YTDLOutPutDir);

                await Production.VerifyDirectoryAndExeIntegrity();

                Settings.PathToVideo = await Production.YouTubeDL(url);
                Settings.DownloadedVideosList.Add(Settings.PathToVideo);

                // Adds To DownloadedVideosList if it is not already containing it,
                if (!Settings.DownloadedVideosList.Contains(Settings.PathToVideo))
                {
                    Settings.DownloadedVideosList.Add(Settings.PathToVideo);
                }

                #region Run FfMpeg
                var parameters = new Dictionary<string, string>();

                string extractedfilename = Path.GetFileName(Settings.PathToVideo);

                parameters["i"] = $@"""{extractedfilename}""";
                parameters["vf"] = "select='gt(scene,0.3)',select=key";
                parameters["vsync"] = "vfr";
                string truePath = Path.GetFullPath(Settings.OutputDir);
                string pictureOutput = $@"""{truePath}/%03d.png""";

                await FFmpegHandler.RunFFMPG(parameters, pictureOutput);
                #endregion

                // ffmpeg has finished, lets copy our mock data
                Mocking.FrontPagePictureLineUp();

                //Serializing.SerializeListOfStringsToXML(Settings.PathToXMLListOfDownloadedVideos, Settings.DownloadedVideosList);

                Settings.Memes = Directory.GetFiles(Settings.DankMemeStashDir, "*.*", SearchOption.AllDirectories);

                #region Face Detection
                var faceDetector = new FaceDetector(0.3F, 0.4F, 0.5F);

                Settings.LogService.LogInformation($"Processing {Settings.Files.Length} images");

                // main loop for detecting faces, placing text where face is not
                for (int fileIndex = 0; fileIndex < Settings.Files.Length; fileIndex++)
                {
                    string file = Settings.Files[fileIndex];

                    Bitmap PicToDetectFacesOn = new(file);

                    FaceDetectionResult[] faceDetectRes = faceDetector.Forward(PicToDetectFacesOn);

                    Rectangle[] facesRectArray = new Rectangle[faceDetectRes.Length];

                    // convert from FaceDetectionResult to Rectangle
                    for (int i = 0; i < faceDetectRes.Length; i++)
                    {
                        facesRectArray[i] = new Rectangle
                        {
                            X = faceDetectRes[i].Rectangle.X,
                            Y = faceDetectRes[i].Rectangle.Y,

                            Width = faceDetectRes[i].Rectangle.Width,
                            Height = faceDetectRes[i].Rectangle.Height
                        };

                    }
                    #endregion

                    #region Data Generation
                    DataGeneration.DecideIfTooMuchFace(file, PicToDetectFacesOn, facesRectArray);

                    if (!Settings.DiscardedBecauseTooMuchFacePictureData.Contains(file))
                    {

                        PictureData PassPictureData = new()
                        {
                            FileName = file,
                            NumberOfBoxes = 2,
                        };

                        for (int AmountOfBoxes = 0; AmountOfBoxes < PassPictureData.NumberOfBoxes; AmountOfBoxes++)
                        {
                            ParamForTextCreation currentParameters = new();

                            currentParameters = DataGeneration.GettextPos(currentParameters, PicToDetectFacesOn, facesRectArray, PassPictureData);

                            currentParameters = DataGeneration.DecideColorGeneration(currentParameters);

                            currentParameters.Font = DataGeneration.PickRandomFont();

                            // picks a random string from the list
                            Random pickAString = new();
                            int pickedString = pickAString.Next(Settings.ListOfText.Count);
                            currentParameters.Text = Settings.ListOfText[pickedString];

                            PassPictureData.BoxParameters.Add(currentParameters);
                        }

                        Settings.PictureDatas.Add(PassPictureData);
                    }
                    #endregion
                }

                Settings.LogService.LogInformation($" Discarded Amount Of Pictures:{Settings.DiscardedBecauseTooMuchFacePictureData.Count}");

                #region Variety Data Generation
                //// Produce varietydata for the current object
                for (int i = 0; i < Settings.PictureDatas.Count; i++)
                {
                    DataGeneration.GenSaturationVariety(Settings.PictureDatas[i]);

                    DataGeneration.GenFontVariety(Settings.PictureDatas[i]);

                    //DataGeneration.GenPlacementOfTextVariety(Settings.PictureDatas[i]);

                    DataGeneration.GenRandomVariety(Settings.PictureDatas[i]);

                    DataGeneration.GenMemePosition(Settings.PictureDatas[i]);
                }
                #endregion

                #region File Production
                //// File Production
                List<Task> productionTasks = [];

                foreach (PictureData picData in Settings.PictureDatas)
                {
                   bool NoBoxes = picData.BoxParameters.All(bp => bp.CurrentBox.Type == BoxType.None);
                    
                    if (!NoBoxes)
                    {
                        Task productionTask = Task.Run(() => Production.ProduceTextPictures(picData));
                        productionTasks.Add(productionTask);
                    }
                }

                await Task.WhenAll(productionTasks);
                #endregion
            }

            #endregion

            #region Variety Picture Output

            if (ProdType == ProductionType.VarietyList)
            {
                if (PicdataObjToVarietize == null)
                {
                    Settings.LogService.LogError("null has been passed to PicdataobjToVarietize");
                }
                else
                {
                    List<Task> productionVarietyTaskList = new();

                    foreach (PictureData picData in PicdataObjToVarietize.Varieties)
                    {
                        Task productionTask = Task.Run(() => Production.ProduceTextPictures(picData));
                        productionVarietyTaskList.Add(productionTask);
                    }

                    await Task.WhenAll(productionVarietyTaskList);
                }

                await Mocking.VarietiesList();
            }
            #endregion

            #region Custom Picture OutPut
            if (ProdType == ProductionType.CustomPicture)
            {
                if (PicdataObjToVarietize == null)
                {
                    Settings.LogService.LogError("Null has been passed to CustomPicture");
                }
                else
                {
                    await Production.ProduceTextPictures(PicdataObjToVarietize);
                    Settings.PictureDatas.Add(PicdataObjToVarietize);
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

            Mocking.SerializePicData();

            Settings.Files = Directory.GetFiles(Settings.OutputDir, "*.*", SearchOption.AllDirectories);

            Settings.LogService.LogInformation("Processing Finished");
            return Settings.PictureDatas;
        }
        #endregion
        public static async Task<List<PictureData>> MockProcess(ProductionType prodtype, string url, List<string> texts, PictureData picdatatoMock = null)
        {
            Settings.ListOfText = texts;

            if (prodtype == ProductionType.FrontPagePictureLineUp)
            {
                await Mocking.SetupFrontPagePictureLineUp();
                Settings.LogService.LogInformation("Mocking of FrontPagePictureLineUp complete");
            }

            if (prodtype == ProductionType.VarietyList)
            {
                // fake variety list
                // this will never let you actually pick one, it will automatically mock to whatever was clicked
                // when the code ran last time
                if (picdatatoMock == null)
                {
                    Settings.LogService.LogError("null has been passed to PicdataobjToVarietize");
                }
                else
                {
                    await Mocking.SetupVarietyDisplay();
                    Settings.LogService.LogInformation("Mocking of Variety List complete");
                }

            }

            if (prodtype == ProductionType.CustomPicture)
            {

                if (picdatatoMock == null)
                {
                    Settings.LogService.LogError("Null has been passed to CustomPicture");
                }
                else
                {
                    await Production.ProduceTextPictures(picdatatoMock);
                    Settings.PictureDatas.Add(picdatatoMock);
                }

            }

            return Settings.PictureDatas;
        }

        static void Main()
        {

        }

    }

}