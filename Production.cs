using BulkThumbnailCreator.Enums;
using BulkThumbnailCreator.PictureObjects;
using ImageMagick;
using System.Diagnostics;
using System.Reflection;
using YoutubeDLSharp;

namespace BulkThumbnailCreator;

public class Production
{
    /// <summary>
    /// Checks if we have our directory/executables  in order
    /// </summary>
    public static async Task VerifyDirectoryAndExeIntegrity(Settings settings)
    {
        string CurrentLoc = Assembly.GetExecutingAssembly().Location;
        string parentDirectory = Directory.GetParent(CurrentLoc).FullName;

        // if dir doesnt exist, make it
        string ExePath = Path.Combine(parentDirectory, "Executables");

        if (!Path.Exists(ExePath))
        {
            Directory.CreateDirectory(ExePath);
        }

        OperatingSystem operatingSystem = Environment.OSVersion;

        string YTDLPDir = Path.Combine(ExePath, "yt-dlp");
        string FfMpegDir = Path.Combine(ExePath, "ffmpeg");

        if (operatingSystem.Platform == PlatformID.Win32NT)
        {
            settings.LogService.LogInformation("Windows OS Detected");
            YTDLPDir += ".exe";
            FfMpegDir += ".exe";
        }
        else if (operatingSystem.Platform == PlatformID.Unix)
        {
            settings.LogService.LogInformation("Unix OS Detected");
        }

        settings.LogService.LogInformation($"Current Location: {CurrentLoc}");
        settings.LogService.LogInformation($"Parent Directory: {parentDirectory}");
        settings.LogService.LogInformation($"New Path: {ExePath}");

        if (File.Exists(YTDLPDir))
        {
            settings.LogService.LogInformation($"YTDLP Path: {YTDLPDir}");
        }
        else
        {
            settings.LogService.LogError("yt-dlp was not found");
            settings.LogService.LogInformation("Will Try To Download yt-dlp");
            await YoutubeDLSharp.Utils.DownloadYtDlp(ExePath);

            if (File.Exists(YTDLPDir))
            {
                settings.LogService.LogInformation("Successfully downloaded yt-dlp");
            }
            else
            {
                settings.LogService.LogError("Failed to download yt-dlp");
            }

        }
        settings.YTDLPDir = YTDLPDir;

        if (File.Exists(FfMpegDir))
        {
            settings.LogService.LogInformation($"FFmpeg Path: {FfMpegDir}");
        }
        else
        {
            // we didnt find ffmpeg
            settings.LogService.LogError("ffmpeg was not found");

            // so will download it
            settings.LogService.LogInformation("Will Try To Download ffmpeg");

            //attempts to download the file to local dir
            await YoutubeDLSharp.Utils.DownloadFFmpeg(ExePath);

            if (File.Exists(FfMpegDir))
            {
                settings.LogService.LogInformation("File has been successfully downloaded");
            }
            else
            {
                settings.LogService.LogInformation("Download of FFMPEG has failed.");
            }

        }

        settings.FfmpegDir = FfMpegDir;

        string ytdlDir = Path.Combine(parentDirectory, "YTDL");

        if (Directory.Exists(ytdlDir))
        {
            settings.LogService.LogInformation($"YTDL Dir found:{ytdlDir}");
        }
        else
        {
            settings.LogService.LogError("YTDL Dir was not found");
            Directory.CreateDirectory(ytdlDir);
            settings.LogService.LogInformation($"Dir Created at:{ytdlDir}");
        }
        settings.YTDLOutPutDir = ytdlDir;

        SetExecutePermission(YTDLPDir, settings);
        SetExecutePermission(FfMpegDir, settings);
    }

    public static void SetExecutePermission(string filePath, Settings settings)
    {
        OperatingSystem operatingSystem = Environment.OSVersion;

        if (operatingSystem.Platform == PlatformID.Win32NT)
        {
            return;
        }
        else if (operatingSystem.Platform == PlatformID.Unix)
        {
            settings.LogService.LogInformation("Unix OS Detected");
            settings.LogService.LogInformation($"Attempting to set execute permission on {filePath}");
        }

        try
        {
            Process chmodProcess = new()
            {
                StartInfo =
        {
            FileName = "chmod",
            Arguments = $"+x {filePath}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
            };

            chmodProcess.Start();
            chmodProcess.WaitForExit();

            string output = chmodProcess.StandardOutput.ReadToEnd();
            string error = chmodProcess.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(output))
            {
                settings.LogService.LogInformation($"chmod output: {output}");
            }

            if (!string.IsNullOrEmpty(error))
            {
                settings.LogService.LogError($"chmod error: {error}");
            }
        }
        catch (Exception ex)
        {
            settings.LogService.LogError($"Error setting execute permission: {ex.Message}");
        }
    }

    /// <summary>
    /// Instantiates a YoutubeDL and downloads the specified string
    /// </summary>
    /// <param name="URL">the specified link URL to download</param>
    /// <returns>returns the path to the downloaded video</returns>
    public static async Task<string> YouTubeDL(string URL, Settings settings)
    {
        var ytdl = new YoutubeDL
        {
            // set paths
            YoutubeDLPath = settings.YTDLPDir,
            FFmpegPath = settings.FfmpegDir,
            OutputFolder = settings.YTDLOutPutDir,
        };

        // downloads specified video from youtube if it does not already exist.
        RunResult<string> res;

        if (URL == null)
        {
            settings.LogService.LogError("URL has been passed as null to YTDL");
            throw new ArgumentNullException(nameof(URL));

        }
        else
        {
            settings.LogService.LogInformation($"Attempting download of: {URL}");
            res = await ytdl.RunVideoDownload(url: URL);
        }

        settings.LogService.LogInformation("Download Success:" + res.Success.ToString());

        // sets BTC to run on the recently downloaded file res.data is the returned path.
        return res.Data;
    }

    public static void CreateDirectories(string outputDir, string TextAdded, string YTDL, Settings settings)
    {
        if (!Directory.Exists(outputDir))
        {
            settings.LogService.LogInformation($"{outputDir} Directory was missing, will be created");
            Directory.CreateDirectory(outputDir);
        }
        if (!Directory.Exists(YTDL))
        {
            settings.LogService.LogInformation($"{TextAdded} Directory was missing, will be created");
            Directory.CreateDirectory(TextAdded);
        }
        if (!Directory.Exists(YTDL))
        {
            settings.LogService.LogInformation($"{YTDL} Directory was missing, will be created");
            Directory.CreateDirectory(YTDL);
        }

        //// Build Directory structure for Mocking process
        //string CreateMockDir = "..";

        //CreateMockDir = Path.Combine(CreateMockDir, "Mocking");

        //if (!Directory.Exists(CreateMockDir))
        //{
        //    Directory.CreateDirectory(CreateMockDir);
        //    settings.LogService.LogInformation($"{CreateMockDir} Created");
        //}

        //string FrontPageLineupMockDir = Path.Combine(CreateMockDir, "FrontpagePictureLineUp");
        //if (!Directory.Exists(FrontPageLineupMockDir))
        //{
        //    Directory.CreateDirectory(FrontPageLineupMockDir);
        //    settings.LogService.LogInformation($"{FrontPageLineupMockDir} Created");
        //}

        //string outputMockDir = Path.Combine(FrontPageLineupMockDir, settings.OutputDir);
        //if (!Directory.Exists(outputMockDir))
        //{
        //    Directory.CreateDirectory(outputMockDir);
        //}

        //string textaddedMockDir = Path.Combine(FrontPageLineupMockDir, settings.TextAddedDir);
        //if (!Directory.Exists(textaddedMockDir))
        //{
        //    Directory.CreateDirectory(textaddedMockDir);
        //    settings.LogService.LogInformation($"{textaddedMockDir} Created");
        //}
    }

    /// <summary>
    /// Produces Picture using the ImageMagick Library
    /// </summary>
    /// <param name="PicData">PictureDataObject Containing everything needed to create an image</param>
    public static async Task ProduceTextPictures(PictureData PicData, Settings settings)
    {
        string imageName;
        string OutputPath = Path.GetFullPath(settings.TextAddedDir);

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
            Guid guid = Guid.NewGuid();
            OutputPath += "//" + guid + imageName;
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
            OutputPath += $"{varietyof}{imageName}//{PicData.OutPutType}{guid}{imageName}.png";
            PicData.OutPath = OutputPath;
        }
        if (PicData.OutPutType == OutputType.Custom)
        {
            Guid guid = Guid.NewGuid();
            OutputPath += $"{varietyof}{imageName}//{guid}Custom of{imageName}";
            PicData.OutPath = OutputPath;
        }

        MagickImage outputImage = new();

        try
        {
            outputImage = new(Path.GetFullPath(PicData.FileName));
        }
        catch (Exception ex)
        {
            settings.LogService.LogError($"Error in reading image into ImageMagick: {ex.Message}");
        }

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
                MagickImage meme = new(BoxParam.Meme);
                meme.Resize(BoxParam.CurrentBox.Width, BoxParam.CurrentBox.Height);
                outputImage.Composite(meme, BoxParam.CurrentBox.X, BoxParam.CurrentBox.Y, CompositeOperator.Over);
            }
            if (PicData.BoxParameters[Box].Meme == null)
            {
                // Add the caption layer on top of the background image
                var caption = new MagickImage($"caption:{BoxParam.Text}", PicData.ReadSettings);

                int takeX = BoxParam.CurrentBox.X;

                int takeY = BoxParam.CurrentBox.Y;

                outputImage.Composite(caption, takeX, takeY, CompositeOperator.Over);
            }

            settings.LogService.LogInformation($"Picture:{Path.GetFileName(PicData.FileName)}Box Type:{PicData.BoxParameters[Box].CurrentBox.Type} Box: {Box + 1} of {PicData.BoxParameters.Count} has been composited");
        }

        try
        {
            outputImage.Annotate("Bulk Thumbnail Creator", gravity: Gravity.North);
        }
        catch (Exception ex)
        {
            settings.LogService.LogError($"Error in annotation: {ex.Message}");
        }

        outputImage.Quality = 100;

        // outputs the file to the provided path and name
        await Task.Run(() => outputImage.Write(OutputPath));
        settings.LogService.LogInformation($"File Created: {OutputPath}");
    }
}