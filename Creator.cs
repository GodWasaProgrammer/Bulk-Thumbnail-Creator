using System.IO;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using FaceONNX;
using System.Drawing;
using Bulk_Thumbnail_Creator.PictureObjects;
using Bulk_Thumbnail_Creator.Serialization;
using Bulk_Thumbnail_Creator.Interfaces;

namespace Bulk_Thumbnail_Creator
{
    public class Creator
    {
        public static async Task<List<PictureData>> Process(ProductionType ProdType, string url, List<string> texts, ILogService logger, PictureData PicdataObjToVarietize = null)
        {
            Settings.ListOfText = texts;

            Settings.LogService = logger;

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

                //parameters["i"] = $@"""{extractedfilename}""";
                //parameters["vf"] = @"select=gt(scene\,0.3)";
                //parameters["vsync"] = "vfr";
                //string truePath = Path.GetFullPath(Settings.OutputDir);
                //string pictureOutput = $@"""{truePath}/%03d.png""";

                // works 
                //parameters["i"] = $@"""{extractedfilename}""";
                //parameters["vf"] = "select='gt(scene,0.3)'";
                //parameters["vsync"] = "vfr";
                //string truePath = Path.GetFullPath(Settings.OutputDir);
                //string pictureOutput = $@"""{truePath}/%03d.png""";

                parameters["i"] = $@"""{extractedfilename}""";
                parameters["vf"] = "select='gt(scene,0.3)',select=key";
                parameters["vsync"] = "vfr";
                string truePath = Path.GetFullPath(Settings.OutputDir);
                string pictureOutput = $@"""{truePath}/%03d.png""";


                FFmpegHandler.RunFFMPG(parameters, pictureOutput);
                #endregion

                Serializing.SerializeListOfStringsToXML(Settings.PathToXMLListOfDownloadedVideos, Settings.DownloadedVideosList);

                Settings.Files = Directory.GetFiles(Settings.OutputDir, "*.*", SearchOption.AllDirectories);

                Settings.Memes = Directory.GetFiles(Settings.DankMemeStashDir, "*.*", SearchOption.AllDirectories);

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
                        var facedetection = faceDetectRes[i];

                        facesRectArray[i] = new Rectangle();

                        facesRectArray[i].X = faceDetectRes[i].Rectangle.X;
                        facesRectArray[i].Y = faceDetectRes[i].Rectangle.Y;

                        facesRectArray[i].Width = faceDetectRes[i].Rectangle.Width;
                        facesRectArray[i].Height = faceDetectRes[i].Rectangle.Height;
                    }

                    DataGeneration.DecideIfTooMuchFace(file, PicToDetectFacesOn, facesRectArray);

                    if (!Settings.DiscardedBecauseTooMuchFacePictureData.Contains(file))
                    {

                        PictureData PassPictureData = new()
                        {
                            FileName = file,
                            //ParamForTextCreation = currentParameters,
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

                }

                Settings.LogService.LogInformation($" Discarded Amount Of Pictures:{Settings.DiscardedBecauseTooMuchFacePictureData.Count}");

                //// Produce varietydata for the current object
                for (int i = 0; i < Settings.PictureDatas.Count; i++)
                {
                    var input = Settings.PictureDatas[i];

                    DataGeneration.GenSaturationVariety(Settings.PictureDatas[i]);

                    DataGeneration.GenFontVariety(Settings.PictureDatas[i]);

                  // DataGeneration.GenPlacementOfTextVariety(input);

                   // DataGeneration.GenRandomVariety(input);

                    // DataGeneration.GenMemePosition(input);
                }

                // actual file output
                for (int i = 0; i < Settings.PictureDatas.Count; i++)
                {
                    string PicObjPath = Settings.PictureDatas[i].FileName;

                    if (!Settings.DiscardedBecauseTooMuchFacePictureData.Contains(PicObjPath))
                    {
                        Production.ProduceTextPictures(Settings.PictureDatas[i]);
                    }

                }

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
                    Parallel.For(0, PicdataObjToVarietize.Varieties.Count, i =>
                    {
                        Production.ProduceTextPictures(PicdataObjToVarietize.Varieties[i]);
                    });

                }

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
                    Bitmap srcpic = new(PicdataObjToVarietize.FileName);

                    for (int numberofBoxes = 0; numberofBoxes < PicdataObjToVarietize.NumberOfBoxes; numberofBoxes++)
                    {
                        PicdataObjToVarietize.BoxParameters[numberofBoxes] = DataGeneration.CalculateBoxData(PicdataObjToVarietize.BoxParameters[numberofBoxes].CurrentBox, srcpic, PicdataObjToVarietize.BoxParameters[numberofBoxes]);
                    }

                    //PicdataObjToVarietize.ParamForTextCreation = DataGeneration.CalculateBoxData(PicdataObjToVarietize.ParamForTextCreation.CurrentBox, srcpic, PicdataObjToVarietize.ParamForTextCreation);

                    Production.ProduceTextPictures(PicdataObjToVarietize);
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

            #region Picdata serialization

            //using (StreamWriter streamWriter = new("Picturedatas.xml"))
            //{
            //    foreach (var PictureData in BTCSettings.PictureDatas)
            //    {
            //        Serializing.SerializePictureData(streamWriter, PictureData);

            //    }
            //    BTCSettings.Logger.LogInformation("PictureDatas.xml Serialized from PictureData");
            //}

            Settings.LogService.LogInformation("Processing Finished");
            return Settings.PictureDatas;
        }
        #endregion

        static void Main()
        {

        }

    }

}