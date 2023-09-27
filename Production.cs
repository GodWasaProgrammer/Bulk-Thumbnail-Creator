﻿using ImageMagick;
using System;
using System.IO;
using System.Threading.Tasks;
using YoutubeDLSharp;
using System.Drawing;
using Bulk_Thumbnail_Creator.Enums;
using Bulk_Thumbnail_Creator.PictureObjects;
using System.Reflection;

namespace Bulk_Thumbnail_Creator
{
    public class Production
    {
        /// <summary>
        /// Instantiates a YoutubeDL and downloads the specified string
        /// </summary>
        /// <param name="URL">the specified link URL to download</param>
        /// <returns>returns the path to the downloaded video</returns>
        public static async Task<string> YouTubeDL(string URL)
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

            BTCSettings.Logger.LogInformation($"Current Location: {CurrentLoc}");
            BTCSettings.Logger.LogInformation($"Parent Directory: {parentDirectory}");
            BTCSettings.Logger.LogInformation($"New Path: {ExePath}");

            if (File.Exists(YTDLPDir))
            {
                BTCSettings.Logger.LogInformation($"YTDLP Path: {YTDLPDir}");
            }
            else
            {
                BTCSettings.Logger.LogError("yt-dlp.exe was not found");
                BTCSettings.Logger.LogInformation("Will Try To Download yt-dlp");
                await YoutubeDLSharp.Utils.DownloadYtDlp(ExePath);

                if (File.Exists(YTDLPDir))
                {
                    BTCSettings.Logger.LogInformation("Successfully downloaded yt-dlp");
                }
                else
                {
                    BTCSettings.Logger.LogError("Failed to download yt-dlp");
                }

            }

            if (File.Exists(FfMpegDir))
            {
                BTCSettings.Logger.LogInformation($"FFmpeg Path: {FfMpegDir}");
            }
            else
            {
                // we didnt find ffmpeg
                BTCSettings.Logger.LogError("ffmpeg.exe was not found");

                // so will download it
                BTCSettings.Logger.LogInformation("Will Try To Download ffmpeg");

                //attempts to download the file to local dir
                await YoutubeDLSharp.Utils.DownloadFFmpeg(ExePath);

                if (File.Exists(FfMpegDir))
                {
                    BTCSettings.Logger.LogInformation("File has been successfully downloaded");
                }
                else
                {
                    BTCSettings.Logger.LogInformation("Download of FFMPEG has failed.");
                }

            }
            BTCSettings.FfmpegDir = FfMpegDir;

            string ytdlDir = Path.Combine(parentDirectory, "YTDL");

            if (Directory.Exists(ytdlDir))
            {
                BTCSettings.Logger.LogInformation($"YTDL Dir found:{ytdlDir}");
            }
            else
            {
                BTCSettings.Logger.LogError("YTDL Dir was not found");
                Directory.CreateDirectory(ytdlDir);
            }

            var ytdl = new YoutubeDL
            {
                // set paths
                YoutubeDLPath = YTDLPDir,
                FFmpegPath = FfMpegDir,
                OutputFolder = ytdlDir,
            };

            // downloads specified video from youtube if it does not already exist.
            RunResult<string> res;

            if (URL == null)
            {
                BTCSettings.Logger.LogError("URL has been passed as null to YTDL");
                throw new ArgumentNullException(nameof(URL));

            }
            else
            {
                BTCSettings.Logger.LogInformation($"Attempting download of: {URL}");
                res = await ytdl.RunVideoDownload(url: URL);
            }

            BTCSettings.Logger.LogInformation("Download Success:" + res.Success.ToString());

            // sets BTC to run on the recently downloaded file res.data is the returned path.
            return res.Data;
        }

        public static void CreateDirectories(string outputDir, string TextAdded, string YTDL)
        {
            if (!Directory.Exists(outputDir))
            {
                BTCSettings.Logger.LogInformation($"{outputDir} Directory was missing, will be created");
                Directory.CreateDirectory(outputDir);
            }
            if (!Directory.Exists(YTDL))
            {
                BTCSettings.Logger.LogInformation($"{TextAdded} Directory was missing, will be created");
                Directory.CreateDirectory(TextAdded);
            }
            if (!Directory.Exists(YTDL))
            {
                BTCSettings.Logger.LogInformation($"{YTDL} Directory was missing, will be created");
                Directory.CreateDirectory(YTDL);
            }

        }

        private static int counterRandomOutPut;

        /// <summary>
        /// Produces Picture using the ImageMagick Library
        /// </summary>
        /// <param name="PicData">PictureDataObject Containing everything needed to create an image</param>
        public static void ProduceTextPictures(PictureData PicData)
        {
            string imageName;
            string OutputPath = Path.GetFullPath(BTCSettings.TextAddedDir);

            imageName = Path.GetFileName(PicData.FileName);
            DateTime dateTime = DateTime.Now;
            string trimDateTime = dateTime.ToString();
            trimDateTime = trimDateTime.Replace(":", "");
            string varietyof = "//variety of ";

            // if not main type, we will make a directory for files to be written in
            if (PicData.OutputType != OutputType.Main)
            {
                Directory.CreateDirectory(OutputPath + varietyof + Path.GetFileName(PicData.FileName));
            }

            if (PicData.OutputType == OutputType.Main)
            {
                OutputPath += "//" + trimDateTime + imageName;
                PicData.OutPath = OutputPath;
            }
            if (PicData.OutputType == OutputType.BoxPositionVariety)
            {
                OutputPath += $"{varietyof}{imageName}//{PicData.ParamForTextCreation.CurrentBox}.png";
                PicData.OutPath = OutputPath;
            }
            if (PicData.OutputType == OutputType.SaturationVariety)
            {
                OutputPath += $"{varietyof}{imageName}//{PicData.ParamForTextCreation.FillColor.Saturation}.png";
                PicData.OutPath = OutputPath;
            }
            if (PicData.OutputType == OutputType.FontVariety)
            {
                OutputPath += $"{varietyof}{imageName}//{Path.GetFileNameWithoutExtension(PicData.ReadSettings.Font)}.png";
                PicData.OutPath = OutputPath;
            }
            if (PicData.OutputType == OutputType.RandomVariety)
            {
                counterRandomOutPut++;
                OutputPath += $"{varietyof}{imageName}//{counterRandomOutPut}.png";
                PicData.OutPath = OutputPath;
            }
            if (PicData.OutputType == OutputType.Dankness)
            {
                OutputPath += $"{varietyof}{imageName}//{PicData.Dankbox}{trimDateTime}{imageName}.png";
                PicData.OutPath = OutputPath;
            }
            if (PicData.OutputType == OutputType.Custom)
            {
                OutputPath += $"{varietyof}{imageName}//{trimDateTime}Custom of{imageName}.png";
                PicData.OutPath = OutputPath;
            }

            using MagickImage outputImage = new(Path.GetFullPath(PicData.FileName));

            using var caption = new MagickImage($"caption:{PicData.ParamForTextCreation.Text}", PicData.ReadSettings);

            // Add the caption layer on top of the background image
            if (PicData.ParamForTextCreation.Boxes.TryGetValue(PicData.ParamForTextCreation.CurrentBox, out Rectangle value))
            {
                int takeX = value.X;

                int takeY = value.Y;

                outputImage.Composite(caption, takeX, takeY, CompositeOperator.Over);
            }

            outputImage.Annotate("Bulk Thumbnail Creator", gravity: Gravity.North);

            if (PicData.OutputType == OutputType.Dankness)
            {
                MagickImage MemeToPutOnPic = new(PicData.Meme);

                Rectangle ReadPosition = PicData.ParamForTextCreation.Boxes[PicData.Dankbox];

                int posX = ReadPosition.X;
                int posY = ReadPosition.Y;

                outputImage.Composite(MemeToPutOnPic, posX, posY, CompositeOperator.Over);
            }
            outputImage.Quality = 100;

            // outputs the file to the provided path and name
            outputImage.Write(OutputPath);

        }

    }

}