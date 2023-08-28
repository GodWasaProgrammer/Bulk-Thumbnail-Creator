﻿using System.IO;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using FaceONNX;
using System.Drawing;
using Bulk_Thumbnail_Creator.PictureObjects;
using System.Runtime.InteropServices;

namespace Bulk_Thumbnail_Creator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //argv0 = process name on cli call

            if (args[1] == "front")
            {
                var texts = new List<string>();
                texts.Add(args[3]);
                texts.Add(args[4]);
                texts.Add(args[5]);
                texts.Add(args[6]);
                await Process(ProductionType.FrontPagePictureLineUp, args[2], texts);
            }

        }

        public static async Task<List<PictureData>> Process(ProductionType ProdType, string url, List<string> texts, PictureData PicdataObjToVarietize = null)
        {
            BTCSettings.ListOfText = texts;

            // creates our 3 dirs to push out unedited thumbnails, and the edited thumbnails and also a path for where the downloaded youtube clips goes.
            Logic.CreateDirectories(BTCSettings.OutputDir, BTCSettings.TextAddedDir, BTCSettings.YoutubeDLDir);

            if (ProdType == ProductionType.FrontPagePictureLineUp)
            {
                BTCSettings.PathToVideo = await Logic.YouTubeDL(url);
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

                Logic.SerializeListOfStringsToXML(BTCSettings.PathToXMLListOfDownloadedVideos, BTCSettings.DownloadedVideosList);

                BTCSettings.Files = Directory.GetFiles(BTCSettings.OutputDir, "*.*", SearchOption.AllDirectories);

                BTCSettings.Memes = Directory.GetFiles(BTCSettings.DankMemeStashDir, "*.*", SearchOption.AllDirectories);

                var faceDetector = new FaceDetector(0.95f, 0.5f);

                Console.WriteLine($"Processing {BTCSettings.Files.Length} images");

                // main loop for detecting faces, placing text where face is not
                for (int fileIndex = 0; fileIndex < BTCSettings.Files.Length; fileIndex++)
                {
                    string file = BTCSettings.Files[fileIndex];

                    Bitmap PicToDetectFacesOn = new(file);

                    Rectangle[] detectedFacesRectArray = faceDetector.Forward(PicToDetectFacesOn);

                    Logic.DecideIfTooMuchFace(file, PicToDetectFacesOn, detectedFacesRectArray);

                    if (!BTCSettings.DiscardedBecauseTooMuchFacePictureData.Contains(file))
                    {
                        ParamForTextCreation currentParameters = new();

                        currentParameters = Logic.GettextPos(currentParameters, PicToDetectFacesOn, detectedFacesRectArray);

                        currentParameters = Logic.DecideColorGeneration(currentParameters);

                        currentParameters.Font = Logic.PickRandomFont();

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

                }
                
                // Produce varietydata for the current object
                Parallel.For (0, BTCSettings.PictureDatas.Count, i =>
                {
                    var input = BTCSettings.PictureDatas[i];

                    Logic.ProduceSaturationVarietyData(input);

                    Logic.ProduceFontVarietyData(input);

                    Logic.ProducePlacementOfTextVarietyData(input);

                    Logic.ProduceRandomVarietyData(input);

                    Logic.ProduceMemeDanknessData(input);
                });

                // actual file output
                Parallel.For(0, BTCSettings.PictureDatas.Count, i =>
                {
                    Logic.ProduceTextPictures(BTCSettings.PictureDatas[i]);
                });

            }
            
            if (ProdType == ProductionType.VarietyList)
            {
                //TODO null check

                if (PicdataObjToVarietize ==  null) 
                {
                    Console.WriteLine("Null has been passed to PicdataobjToVarietize");
                }
                else
                {
                    Parallel.For(0, PicdataObjToVarietize.Varieties.Count, i =>
                    {
                        Logic.ProduceTextPictures(PicdataObjToVarietize.Varieties[i]);
                    });

                }

            }

            #region Make Showcase Video
            //Dictionary<string, string> paramToMakeVideoOfResult = new Dictionary<string, string>();
            //paramToMakeVideoOfResult["framerate"] = "2";
            //paramToMakeVideoOfResult["i"] = $@"""{Path.GetFullPath(BTCSettings.TextAddedDir)}/%03d.png""";
            //string getTruePath = Path.GetFullPath(BTCSettings.TextAddedDir);
            //string showCaseVideoOutPut = $@"""{getTruePath}/showcase.mp4""";

            //FFmpegHandler.RunFFMPG(paramToMakeVideoOfResult, showCaseVideoOutPut);
            #endregion

            await Console.Out.WriteLineAsync("Processing Finished");
            return BTCSettings.PictureDatas;
        }

    }

}
