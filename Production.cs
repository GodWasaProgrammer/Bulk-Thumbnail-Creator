﻿using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.PictureObjects;
using ImageMagick;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using YoutubeDLSharp;

namespace Bulk_Thumbnail_Creator
{
    public class Production
    {

        /// <summary>
        /// Checks if we have our directory/executables  in order
        /// </summary>
        public static async Task VerifyDirectoryAndExeIntegrity()
        {
            string CurrentLoc = Assembly.GetExecutingAssembly().Location;
            string parentDirectory = Directory.GetParent(CurrentLoc).FullName;

            // if dir doesnt exist, make it
            string ExePath = Path.Combine(parentDirectory, "Executables");

            if (!Path.Exists(ExePath))
            {
                Directory.CreateDirectory(ExePath);
            }

            string YTDLPDir = Path.Combine(ExePath, "yt-dlp.exe");
            string FfMpegDir = Path.Combine(ExePath, "ffmpeg.exe");

            Settings.LogService.LogInformation($"Current Location: {CurrentLoc}");
            Settings.LogService.LogInformation($"Parent Directory: {parentDirectory}");
            Settings.LogService.LogInformation($"New Path: {ExePath}");

            if (File.Exists(YTDLPDir))
            {
                Settings.LogService.LogInformation($"YTDLP Path: {YTDLPDir}");
            }
            else
            {
                Settings.LogService.LogError("yt-dlp.exe was not found");
                Settings.LogService.LogInformation("Will Try To Download yt-dlp");
                await YoutubeDLSharp.Utils.DownloadYtDlp(ExePath);

                if (File.Exists(YTDLPDir))
                {
                    Settings.LogService.LogInformation("Successfully downloaded yt-dlp");
                }
                else
                {
                    Settings.LogService.LogError("Failed to download yt-dlp");
                }

            }
            Settings.YTDLPDir = YTDLPDir;

            if (File.Exists(FfMpegDir))
            {
                Settings.LogService.LogInformation($"FFmpeg Path: {FfMpegDir}");
            }
            else
            {
                // we didnt find ffmpeg
                Settings.LogService.LogError("ffmpeg.exe was not found");

                // so will download it
                Settings.LogService.LogInformation("Will Try To Download ffmpeg");

                //attempts to download the file to local dir
                await YoutubeDLSharp.Utils.DownloadFFmpeg(ExePath);

                if (File.Exists(FfMpegDir))
                {
                    Settings.LogService.LogInformation("File has been successfully downloaded");
                }
                else
                {
                    Settings.LogService.LogInformation("Download of FFMPEG has failed.");
                }

            }

            Settings.FfmpegDir = FfMpegDir;

            string ytdlDir = Path.Combine(parentDirectory, "YTDL");

            if (Directory.Exists(ytdlDir))
            {
                Settings.LogService.LogInformation($"YTDL Dir found:{ytdlDir}");
            }
            else
            {
                Settings.LogService.LogError("YTDL Dir was not found");
                Directory.CreateDirectory(ytdlDir);
                Settings.LogService.LogInformation($"Dir Created at:{ytdlDir}");
            }
            Settings.YTDLOutPutDir = ytdlDir;
        }

        /// <summary>
        /// Instantiates a YoutubeDL and downloads the specified string
        /// </summary>
        /// <param name="URL">the specified link URL to download</param>
        /// <returns>returns the path to the downloaded video</returns>
        public static async Task<string> YouTubeDL(string URL)
        {
            var ytdl = new YoutubeDL
            {
                // set paths
                YoutubeDLPath = Settings.YTDLPDir,
                FFmpegPath = Settings.FfmpegDir,
                OutputFolder = Settings.YTDLOutPutDir,
            };

            // downloads specified video from youtube if it does not already exist.
            RunResult<string> res;

            if (URL == null)
            {
                Settings.LogService.LogError("URL has been passed as null to YTDL");
                throw new ArgumentNullException(nameof(URL));

            }
            else
            {
                Settings.LogService.LogInformation($"Attempting download of: {URL}");
                res = await ytdl.RunVideoDownload(url: URL);
            }

            Settings.LogService.LogInformation("Download Success:" + res.Success.ToString());

            // sets BTC to run on the recently downloaded file res.data is the returned path.
            return res.Data;
        }

        public static void CreateDirectories(string outputDir, string TextAdded, string YTDL)
        {
            if (!Directory.Exists(outputDir))
            {
                Settings.LogService.LogInformation($"{outputDir} Directory was missing, will be created");
                Directory.CreateDirectory(outputDir);
            }
            if (!Directory.Exists(YTDL))
            {
                Settings.LogService.LogInformation($"{TextAdded} Directory was missing, will be created");
                Directory.CreateDirectory(TextAdded);
            }
            if (!Directory.Exists(YTDL))
            {
                Settings.LogService.LogInformation($"{YTDL} Directory was missing, will be created");
                Directory.CreateDirectory(YTDL);
            }

            // Build Directory structure for Mocking process
            string CreateMockDir = "..";

            CreateMockDir = Path.Combine(CreateMockDir, "Mocking");

            if (!Directory.Exists(CreateMockDir))
            {
                Directory.CreateDirectory(CreateMockDir);
                Settings.LogService.LogInformation($"{CreateMockDir} Created");
            }

            string FrontPageLineupMockDir = Path.Combine(CreateMockDir, "FrontpagePictureLineUp");
            if (!Directory.Exists(FrontPageLineupMockDir))
            {
                Directory.CreateDirectory(FrontPageLineupMockDir);
                Settings.LogService.LogInformation($"{FrontPageLineupMockDir} Created");
            }

            string outputMockDir = Path.Combine(FrontPageLineupMockDir, Settings.OutputDir);
            if (!Directory.Exists(outputMockDir))
            {
                Directory.CreateDirectory(outputMockDir);
            }

            string textaddedMockDir = Path.Combine(FrontPageLineupMockDir, Settings.TextAddedDir);
            if (!Directory.Exists(textaddedMockDir))
            {
                Directory.CreateDirectory(textaddedMockDir);
                Settings.LogService.LogInformation($"{textaddedMockDir} Created");
            }

        }

        /// <summary>
        /// Produces Picture using the ImageMagick Library
        /// </summary>
        /// <param name="PicData">PictureDataObject Containing everything needed to create an image</param>
        public static async Task ProduceTextPictures(PictureData PicData)
        {
            string imageName;
            string OutputPath = Path.GetFullPath(Settings.TextAddedDir);

            imageName = Path.GetFileName(PicData.FileName);
            DateTime dateTime = DateTime.Now;
            string trimDateTime = dateTime.ToString();
            trimDateTime = trimDateTime.Replace(":", "");
            string varietyof = "//variety of ";

            // if not main type, we will make a directory for files to be written in
            if (PicData.OutPutType != OutputType.Main)
            {
                Directory.CreateDirectory(OutputPath + varietyof + Path.GetFileName(PicData.FileName));
            }

            if (PicData.OutPutType == OutputType.Main)
            {
                OutputPath += "//" + trimDateTime + imageName;
                PicData.OutPath = OutputPath;
            }
            if (PicData.OutPutType == OutputType.BoxPositionVariety)
            {
                Guid guid = Guid.NewGuid();
                OutputPath += $"{varietyof}{imageName}//{guid}.png";
                PicData.OutPath = OutputPath;
            }
            if (PicData.OutPutType == OutputType.SaturationVariety)
            {
                Guid guid = Guid.NewGuid();
                OutputPath += $"{varietyof}{imageName}//{guid}.png";
                PicData.OutPath = OutputPath;
            }
            if (PicData.OutPutType == OutputType.FontVariety)
            {
                Guid guid = Guid.NewGuid();
                OutputPath += $"{varietyof}{imageName}//{guid}.png";
                PicData.OutPath = OutputPath;
            }
            if (PicData.OutPutType == OutputType.RandomVariety)
            {
                Guid guid = Guid.NewGuid();
                OutputPath += $"{varietyof}{imageName}//{guid}.png";
                PicData.OutPath = OutputPath;
            }
            if (PicData.OutPutType == OutputType.MemeVariety)
            {
                Guid guid = Guid.NewGuid();
                OutputPath += $"{varietyof}{imageName}//{guid}{PicData.OutPutType}{trimDateTime}{imageName}.png";
                PicData.OutPath = OutputPath;
            }
            if (PicData.OutPutType == OutputType.Custom)
            {
                OutputPath += $"{varietyof}{imageName}//{trimDateTime}Custom of{imageName}";
                PicData.OutPath = OutputPath;
            }

            using MagickImage outputImage = new(Path.GetFullPath(PicData.FileName));

            for (int Box = 0; Box < PicData.BoxParameters.Count; Box++)
            {
                if (PicData.BoxParameters[Box].CurrentBox.Type == BoxType.None)
                {
                    break;
                }

                ParamForTextCreation BoxParam = PicData.BoxParameters[Box];
                PicData.MakeTextSettings(PicData.BoxParameters[Box]);


                if (PicData.OutPutType == OutputType.MemeVariety && PicData.BoxParameters[Box].Meme != null)
                {
                    using MagickImage meme = new(BoxParam.Meme);
                    meme.Resize(BoxParam.CurrentBox.Width, BoxParam.CurrentBox.Height);
                    outputImage.Composite(meme, BoxParam.CurrentBox.X, BoxParam.CurrentBox.Y, CompositeOperator.Over);
                }
                if(PicData.BoxParameters[Box].Meme == null)
                {
                    // Add the caption layer on top of the background image
                    var caption = new MagickImage($"caption:{BoxParam.Text}", PicData.ReadSettings);

                    int takeX = BoxParam.CurrentBox.X;

                    int takeY = BoxParam.CurrentBox.Y;

                    outputImage.Composite(caption, takeX, takeY, CompositeOperator.Over);
                }

                Settings.LogService.LogInformation($"Picture:{Path.GetFileName(PicData.FileName)}Box Type:{PicData.BoxParameters[Box].CurrentBox.Type} Box: {Box + 1} of {PicData.BoxParameters.Count} has been composited");
            }

            outputImage.Annotate("Bulk Thumbnail Creator", gravity: Gravity.North);
            outputImage.Quality = 100;

            // outputs the file to the provided path and name
            await Task.Run(() => outputImage.Write(OutputPath));
            Settings.LogService.LogInformation($"File Created: {OutputPath}");
        }

    }

}