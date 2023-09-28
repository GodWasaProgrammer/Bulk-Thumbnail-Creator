using System.IO;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using FaceONNX;
using System.Drawing;
using Bulk_Thumbnail_Creator.PictureObjects;
using Bulk_Thumbnail_Creator.Serialization;

namespace Bulk_Thumbnail_Creator
{
    public class Program
    {
        public static async Task<List<PictureData>> Process(ProductionType ProdType, string url, List<string> texts, PictureData PicdataObjToVarietize = null)
        {
            BTCSettings.ListOfText = texts;

            // creates our 3 dirs to push out unedited thumbnails, and the edited thumbnails and also a path for where the downloaded youtube clips goes.
            Production.CreateDirectories(BTCSettings.OutputDir, BTCSettings.TextAddedDir, BTCSettings.YoutubeDLDir);
            
            #region Front Page Picture Line Up Output
            if (ProdType == ProductionType.FrontPagePictureLineUp)
            {
                BTCSettings.PathToVideo = await Production.YouTubeDL(url);
                BTCSettings.DownloadedVideosList.Add(BTCSettings.PathToVideo);

                // Adds To DownloadedVideosList if it is not already containing it,
                // this has some issue where it still writes it...
                if (!BTCSettings.DownloadedVideosList.Contains(BTCSettings.PathToVideo))
                {
                    BTCSettings.DownloadedVideosList.Add(BTCSettings.PathToVideo);
                }

                #region Run FfMpeg
                var parameters = new Dictionary<string, string>();

                string extractedfilename = Path.GetFileName(BTCSettings.PathToVideo);

                parameters["i"] = $@"""{extractedfilename}""";
                parameters["vf"] = @"select=gt(scene\,0.3)";
                parameters["vsync"] = "vfr";
                string truePath = Path.GetFullPath(BTCSettings.OutputDir);
                string pictureOutput = $@"""{truePath}/%03d.png""";

                FFmpegHandler.RunFFMPG(parameters, pictureOutput);
                #endregion

                Serializing.SerializeListOfStringsToXML(BTCSettings.PathToXMLListOfDownloadedVideos, BTCSettings.DownloadedVideosList);

                BTCSettings.Files = Directory.GetFiles(BTCSettings.OutputDir, "*.*", SearchOption.AllDirectories);

                BTCSettings.Memes = Directory.GetFiles(BTCSettings.DankMemeStashDir, "*.*", SearchOption.AllDirectories);

                var faceDetector = new FaceDetector(0.95f, 0.5f);

                BTCSettings.Logger.LogInformation($"Processing {BTCSettings.Files.Length} images");

                // main loop for detecting faces, placing text where face is not
                Parallel.For(0, BTCSettings.Files.Length, fileIndex =>
                {
                    string file = BTCSettings.Files[fileIndex];

                    Bitmap PicToDetectFacesOn = new(file);

                    Rectangle[] detectedFacesRectArray = faceDetector.Forward(PicToDetectFacesOn);

                    DataGeneration.DecideIfTooMuchFace(file, PicToDetectFacesOn, detectedFacesRectArray);

                    if (!BTCSettings.DiscardedBecauseTooMuchFacePictureData.Contains(file))
                    {
                        ParamForTextCreation currentParameters = new();

                        currentParameters = DataGeneration.GettextPos(currentParameters, PicToDetectFacesOn, detectedFacesRectArray);

                        currentParameters = DataGeneration.DecideColorGeneration(currentParameters);

                        currentParameters.Font = DataGeneration.PickRandomFont();

                        // picks a random string from the list
                        Random pickAString = new();
                        int pickedString = pickAString.Next(BTCSettings.ListOfText.Count);
                        currentParameters.Text = BTCSettings.ListOfText[pickedString];

                        PictureData PassPictureData = new()
                        {
                            FileName = file,
                            ParamForTextCreation = currentParameters,
                        };

                        BTCSettings.PictureDatas.Add(PassPictureData);
                    }

                });

                BTCSettings.Logger.LogInformation($" Discarded Amount Of Pictures:{BTCSettings.DiscardedBecauseTooMuchFacePictureData.Count}");

                #region debugsinglefor
                // just here for debug purposes

                //for (int i = 0; i < BTCSettings.PictureDatas.Count; i++)
                //{
                //    var input = BTCSettings.PictureDatas[i];

                //    Logic.ProduceSaturationVarietyData(input);

                //    Logic.ProduceFontVarietyData(input);

                //    Logic.ProducePlacementOfTextVarietyData(input);

                //    Logic.ProduceRandomVarietyData(input);

                //    Logic.ProduceMemePositionData(input);
                //}
                // _-------------------------------------------------------
                #endregion

                //// Produce varietydata for the current object
                Parallel.For(0, BTCSettings.PictureDatas.Count, i =>
                {
                    var input = BTCSettings.PictureDatas[i];

                    DataGeneration.GenSaturationVariety(input);

                    DataGeneration.GenFontVariety(input);

                    DataGeneration.GenPlacementOfTextVariety(input);

                    DataGeneration.GenRandomVariety(input);

                    DataGeneration.GenMemePosition(input);
                });

                // actual file output
                Parallel.For(0, BTCSettings.PictureDatas.Count, i =>
                {
                    string PicObjPath = BTCSettings.PictureDatas[i].FileName;

                    if (!BTCSettings.DiscardedBecauseTooMuchFacePictureData.Contains(PicObjPath))
                    {
                        Production.ProduceTextPictures(BTCSettings.PictureDatas[i]);
                    }

                });

            }
            #endregion

            #region Variety Picture Output
            if (ProdType == ProductionType.VarietyList)
            {
                if (PicdataObjToVarietize == null)
                {
                    BTCSettings.Logger.LogError("null has been passed to PicdataobjToVarietize");
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
                    BTCSettings.Logger.LogError("Null has been passed to CustomPicture");
                }
                else
                {
                    Bitmap srcpic = new(PicdataObjToVarietize.FileName);
                    PicdataObjToVarietize.ParamForTextCreation = DataGeneration.CalculateBoxData(PicdataObjToVarietize.ParamForTextCreation.CurrentBox, srcpic, PicdataObjToVarietize.ParamForTextCreation);

                    Production.ProduceTextPictures(PicdataObjToVarietize);
                    BTCSettings.PictureDatas.Add(PicdataObjToVarietize);
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

            BTCSettings.Logger.LogInformation("Processing Finished");
            return BTCSettings.PictureDatas;
        }
         #endregion
        static void Main()
        {

        }

    }

}