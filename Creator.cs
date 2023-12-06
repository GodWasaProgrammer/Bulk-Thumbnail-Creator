using Bulk_Thumbnail_Creator.PictureObjects;
using FaceONNX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
                string[] outputDirList = Directory.GetFiles(Settings.OutputDir);
                string srcMockFolder = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp");

                foreach (string outputFile in outputDirList)
                {
                    string completePath = Path.Combine(srcMockFolder, outputFile);
                    await Task.Run(() => File.Copy(outputFile, completePath));
                }

                // copy pictures to text added dir
                string sourceDirectory = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp", "text added");

                string[] files = Directory.GetFiles(sourceDirectory);

                foreach (string file in files)
                {
                    await Task.Run(() => File.Copy(file, $"{Settings.TextAddedDir}/{Path.GetFileName(file)}"));
                }

                srcMockFolder = Path.Combine(srcMockFolder, "mockFP.xml");

                List<PictureData> deserializedList;

                // Create a XmlSerializer for the list of PictureData
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<PictureData>));

                using (StreamReader streamReader = new StreamReader(srcMockFolder))
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
                // since this is fake we will not bother with the 

                if (picdatatoMock == null)
                {
                    Settings.LogService.LogError("null has been passed to PicdataobjToVarietize");
                }
                else
                {
                    // copy pictures to text added dir / var dir                    // copy pictures to text added dir / var dir
                    string sourceDirectory = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp", "text added");

                    string[] Directories = Directory.GetDirectories(sourceDirectory);

                    foreach (string dir in Directories)
                    {
                        // this should only have one directory to copy
                        // since we are mocking the variety list

                        string[] files = Directory.GetFiles(dir);

                        string dirname = Path.GetFileName(dir);

                        Directory.CreateDirectory(Path.Combine(Settings.TextAddedDir, dirname));

                        foreach (string file in files)
                        {
                            string outPath = Path.Combine(Settings.TextAddedDir, dirname);
                            outPath = Path.Combine(outPath, Path.GetFileName(file));

                            await Task.Run(() => File.Copy(file, outPath));
                            Settings.LogService.LogInformation($"Copied {file} to {outPath}");
                        }

                    }

                }

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

                // ffmpeg has finished, lets copy our mock data

                // first we will clear the directory of any files
                string mockOutPutDir = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "output");
                string[] mockOutPutDirFiles = Directory.GetFiles(mockOutPutDir);

                foreach (var MockOutPutFile in mockOutPutDirFiles)
                {
                    File.Delete(MockOutPutFile);
                }


                // Mock copying 

                string[] OutPutDirFiles = Directory.GetFiles(Settings.OutputDir);
                Settings.Files = Directory.GetFiles(Settings.OutputDir, "*.*", SearchOption.AllDirectories);

                if (Directory.Exists(mockOutPutDir))
                {
                    foreach (var file in Settings.Files)
                    {
                        string filename = Path.GetFileName(file);
                        string writePath = Path.Combine(mockOutPutDir, filename);
                        File.Copy(file, writePath);
                    }

                }

                //Serializing.SerializeListOfStringsToXML(Settings.PathToXMLListOfDownloadedVideos, Settings.DownloadedVideosList);

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

                DirectoryInfo di = new(Settings.TextAddedDir);

                DirectoryInfo[] di2 = di.GetDirectories();

                foreach (DirectoryInfo dir in di2)
                {
                    string[] files = Directory.GetFiles(dir.FullName);

                    string TargetDirectory = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp", "text added");

                    // add the variety directory to the target directory

                    Directory.CreateDirectory($"{TargetDirectory}/{dir.Name}");

                    TargetDirectory = Path.Combine(TargetDirectory, dir.Name);

                    foreach (string file in files)
                    {
                        string filename = Path.GetFileName(file);
                        await Task.Run(() => File.Copy(file, $"{TargetDirectory}/{filename}"));
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

            #region Picdata serialization & Mock Setup

            XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(List<PictureData>));

            // Open the file for writing or create a new one if it doesn't exist
            using (StreamWriter streamWriter = new StreamWriter("mockFP.xml"))
            {
                // Serialize the entire list at once
                xmlSerializer2.Serialize(streamWriter, Settings.PictureDatas);
            }
            Settings.LogService.LogInformation("Settings.PictureDatas Serialized to mockFP.xml");

            string mockDir = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "mockFP.xml");
            File.Copy("mockFP.xml", mockDir, true);

            // clean up the text added dir of mockfolder
            string mockdir3 = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "text added");

            DirectoryInfo di3 = new(mockdir3);

            foreach (FileInfo file in di3.GetFiles())
            {
                file.Delete();
            }

            // Copy the Text Added Directory to Mock Folder
            string mockdir2 = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "text added");

            string[] Mockfiles = Directory.GetFiles(Settings.TextAddedDir);

            foreach (string file in Mockfiles)
            {
                File.Copy(file, $"{mockdir2}/{Path.GetFileName(file)}", true);
            }

            Settings.Files = Directory.GetFiles(Settings.OutputDir, "*.*", SearchOption.AllDirectories);

            Settings.LogService.LogInformation("Processing Finished");
            return Settings.PictureDatas;
        }
        #endregion

        static void Main()
        {

        }

    }

}