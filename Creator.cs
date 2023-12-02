using System.IO;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using FaceONNX;
using System.Drawing;
using Bulk_Thumbnail_Creator.PictureObjects;
using Bulk_Thumbnail_Creator.Serialization;
using System.Xml.Serialization;
using Serilog.Core;

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
        public static async Task<List<PictureData>> MockProcess(ProductionType prodtype, string url, List<string> texts, PictureData picdatatoMock = null)
        {
            Settings.ListOfText = texts;

            if (prodtype == ProductionType.FrontPagePictureLineUp)
            {
                // pretend to make line up


                // copy pictures to text added dir
                string sourceDirectory = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp", "text added");

                string[] files = Directory.GetFiles(sourceDirectory);

                foreach (string file in files)
                {
                    await Task.Run(() => File.Copy(file, $"{Settings.TextAddedDir}/{Path.GetFileName(file)}"));
                }

                string srcXml = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp");
                srcXml = Path.Combine(srcXml, "mockFP.xml");

                List<PictureData> deserializedList;

                // Create a XmlSerializer for the list of PictureData
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<PictureData>));

                using (StreamReader streamReader = new StreamReader(srcXml))
                {
                    deserializedList = (List<PictureData>)xmlSerializer.Deserialize(streamReader);
                }
                // add the deserialized list to the PictureDatas list
                // this completes the mocking of the process
                 Settings.PictureDatas = deserializedList;
            }

            if (prodtype == ProductionType.VarietyList)
            {
                // fake variety list

                // copy pictures to text added dir / var dir

                // read xml of picdata objects
                // feed that to picdataservice
            }

            if (prodtype == ProductionType.CustomPicture)
            {
                // mock custom pic 
                // copy picture to textadded dir / var dir 
                // read xml of picdata object to mock
            }


            return Settings.PictureDatas;
        }

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

                //parameters["i"] = $@"""{extractedfilename}""";
                //parameters["vf"] = @"select=gt(scene\,0.3)";
                //parameters["vsync"] = "vfr";
                //string truePath = Path.GetFullPath(Settings.OutputDir);
                //string pictureOutput = $@"""{truePath}/%03d.png""";

                parameters["i"] = $@"""{extractedfilename}""";
                parameters["vf"] = "select='gt(scene,0.3)',select=key";
                parameters["vsync"] = "vfr";
                string truePath = Path.GetFullPath(Settings.OutputDir);
                string pictureOutput = $@"""{truePath}/%03d.png""";

                await FFmpegHandler.RunFFMPG(parameters, pictureOutput);
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
                        facesRectArray[i] = new Rectangle
                        {
                            X = faceDetectRes[i].Rectangle.X,
                            Y = faceDetectRes[i].Rectangle.Y,

                            Width = faceDetectRes[i].Rectangle.Width,
                            Height = faceDetectRes[i].Rectangle.Height
                        };

                    }

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

                }

                Settings.LogService.LogInformation($" Discarded Amount Of Pictures:{Settings.DiscardedBecauseTooMuchFacePictureData.Count}");

                //// Produce varietydata for the current object
                for (int i = 0; i < Settings.PictureDatas.Count; i++)
                {

                    DataGeneration.GenSaturationVariety(Settings.PictureDatas[i]);

                    DataGeneration.GenFontVariety(Settings.PictureDatas[i]);

                    //DataGeneration.GenPlacementOfTextVariety(Settings.PictureDatas[i]);

                    DataGeneration.GenRandomVariety(Settings.PictureDatas[i]);

                    DataGeneration.GenMemePosition(Settings.PictureDatas[i]);
                }

                // actual file output
                for (int i = 0; i < Settings.PictureDatas.Count; i++)
                {
                    string PicObjPath = Settings.PictureDatas[i].FileName;

                    if (!Settings.DiscardedBecauseTooMuchFacePictureData.Contains(PicObjPath))
                    {
                        await Production.ProduceTextPictures(Settings.PictureDatas[i]);
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
                    for (int i = 0; i < PicdataObjToVarietize.Varieties.Count; i++)
                    {
                        await Production.ProduceTextPictures(PicdataObjToVarietize.Varieties[i]);
                    }

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

            #region Picdata serialization


            XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(List<PictureData>));

            // Open the file for writing or create a new one if it doesn't exist
            using (StreamWriter streamWriter = new StreamWriter("mockFP.xml"))
            {
                // Serialize the entire list at once
                xmlSerializer2.Serialize(streamWriter, Settings.PictureDatas);
            }
            Settings.LogService.LogInformation("Settings.PictureDatas Serialized to mockFP.xml");

            string mockDir = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "mockFP.xml");
            File.Copy("mockFP.xml", mockDir,true);

            // clean up the text added dir of mockfolder
            string mockdir3 = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "text added");

            DirectoryInfo di3 = new(mockdir3);

            foreach (FileInfo file in di3.GetFiles())
            {
                file.Delete();
            }

            string mockdir2 = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "text added");

            string[] Mockfiles = Directory.GetFiles(Settings.TextAddedDir);

            foreach (string file in Mockfiles)
            {
                File.Copy(file, $"{mockdir2}/{Path.GetFileName(file)}", true);
            }
            
            Settings.LogService.LogInformation("Processing Finished");
            return Settings.PictureDatas;
        }
        #endregion

        static void Main()
        {

        }

    }

}