// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Ignore Spelling: ytdl Exe

namespace BulkThumbnailCreator;

public class Production
{
    /// <summary>
    /// Checks if we have our directory/executables  in order
    /// </summary>
    public static async Task VerifyDirectoryAndExeIntegrity(Settings settings)
    {
        var currentLoc = Assembly.GetExecutingAssembly().Location;
        var parentDirectory = Directory.GetParent(currentLoc).FullName;

        // if dir doesnt exist, make it
        var exePath = Path.Combine(parentDirectory, "Executables");

        if (!Path.Exists(exePath))
        {
            Directory.CreateDirectory(exePath);
        }

        var operatingSystem = Environment.OSVersion;

        var YtdlpDir = Path.Combine(exePath, "yt-dlp");
        var ffMpegDir = Path.Combine(exePath, "ffmpeg");

        if (operatingSystem.Platform == PlatformID.Win32NT)
        {
            await settings.LogService.LogInformation("Windows OS Detected");
            YtdlpDir += ".exe";
            ffMpegDir += ".exe";
        }
        else if (operatingSystem.Platform == PlatformID.Unix)
        {
            await settings.LogService.LogInformation("Unix OS Detected");
        }

        await settings.LogService.LogInformation($"Current Location: {currentLoc}");
        await settings.LogService.LogInformation($"Parent Directory: {parentDirectory}");
        await settings.LogService.LogInformation($"New Path: {exePath}");

        if (File.Exists(YtdlpDir))
        {
            await settings.LogService.LogInformation($"YTDLP Path: {YtdlpDir}");
        }
        else
        {
            await settings.LogService.LogError("yt-dlp was not found");
            await settings.LogService.LogInformation("Will Try To Download yt-dlp");
            await YoutubeDLSharp.Utils.DownloadYtDlp(exePath);

            if (File.Exists(YtdlpDir))
            {
                await settings.LogService.LogInformation("Successfully downloaded yt-dlp");
            }
            else
            {
                await settings.LogService.LogError("Failed to download yt-dlp");
            }
        }
        settings.YTDLPDir = YtdlpDir;

        if (File.Exists(ffMpegDir))
        {
            await settings.LogService.LogInformation($"FFmpeg Path: {ffMpegDir}");
        }
        else
        {
            // we didnt find ffmpeg
            await settings.LogService.LogError("ffmpeg was not found");

            // so will download it
            await settings.LogService.LogInformation("Will Try To Download ffmpeg");

            //attempts to download the file to local dir
            await YoutubeDLSharp.Utils.DownloadFFmpeg(exePath);

            if (File.Exists(ffMpegDir))
            {
                await settings.LogService.LogInformation("File has been successfully downloaded");
            }
            else
            {
                await settings.LogService.LogInformation("Download of FFMPEG has failed.");
            }
        }

        settings.FfmpegDir = ffMpegDir;
        var ytdlDir = Path.Combine(parentDirectory, "YTDL");

        if (Directory.Exists(ytdlDir))
        {
            await settings.LogService.LogInformation($"YTDL Dir found:{ytdlDir}");
        }
        else
        {
            await settings.LogService.LogError("YTDL Dir was not found");
            Directory.CreateDirectory(ytdlDir);
            await settings.LogService.LogInformation($"Dir Created at:{ytdlDir}");
        }
        settings.YTDLOutPutDir = ytdlDir;

        SetExecutePermission(YtdlpDir, settings);
        SetExecutePermission(ffMpegDir, settings);
    }

    public static void SetExecutePermission(string filePath, Settings settings)
    {
        var operatingSystem = Environment.OSVersion;

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

            var output = chmodProcess.StandardOutput.ReadToEnd();
            var error = chmodProcess.StandardError.ReadToEnd();

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
    /// <param name="url">the specified link URL to download</param>
    /// <returns>returns the path to the downloaded video</returns>
    public static async Task<string> YouTubeDL(string url, Settings settings)
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

        if (url == null)
        {
            await settings.LogService.LogError("URL has been passed as null to YTDL");
            throw new ArgumentNullException(nameof(url));
        }
        else
        {
            await settings.LogService.LogInformation($"Attempting download of: {url}");
            res = await ytdl.RunVideoDownload(url: url);
        }

        await settings.LogService.LogInformation("Download Success:" + res.Success.ToString());

        // sets BTC to run on the recently downloaded file res.data is the returned path.
        return res.Data;
    }

    public static void CreateDirectories(string outputDir, string textAdded, string ytdl, Settings settings)
    {
        if (!Directory.Exists(outputDir))
        {
            settings.LogService.LogInformation($"{outputDir} Directory was missing, will be created");
            Directory.CreateDirectory(outputDir);
        }
        if (!Directory.Exists(ytdl))
        {
            settings.LogService.LogInformation($"{textAdded} Directory was missing, will be created");
            Directory.CreateDirectory(textAdded);
        }
        if (!Directory.Exists(ytdl))
        {
            settings.LogService.LogInformation($"{ytdl} Directory was missing, will be created");
            Directory.CreateDirectory(ytdl);
        }
    }

    /// <summary>
    /// Produces Picture using the ImageMagick Library
    /// </summary>
    /// <param name="pictureData">PictureDataObject Containing everything needed to create an image</param>
    public static async Task ProduceTextPictures(PictureData pictureData, Settings settings)
    {
        string imageName;
        var outputPath = Path.GetFullPath(settings.TextAddedDir);

        imageName = Path.GetFileName(pictureData.FileName);
        const string Varietyof = "//varietyof";

        // if not main type, we will make a directory for files to be written in
        if (pictureData.OutPutType != OutputType.Main)
        {
            Directory.CreateDirectory(outputPath + Varietyof + Path.GetFileName(pictureData.FileName));
        }

        if (pictureData.OutPutType == OutputType.Main)
        {
            var guid = Guid.NewGuid();
            outputPath += "//" + guid + imageName;
            pictureData.OutPath = outputPath;
        }
        if (pictureData.OutPutType == OutputType.BoxPositionVariety)
        {
            var guid = Guid.NewGuid();
            outputPath += $"{Varietyof}{imageName}//{guid}.png";
            pictureData.OutPath = outputPath;
        }
        if (pictureData.OutPutType == OutputType.SaturationVariety)
        {
            var guid = Guid.NewGuid();
            outputPath += $"{Varietyof}{imageName}//{guid}.png";
            pictureData.OutPath = outputPath;
        }
        if (pictureData.OutPutType == OutputType.FontVariety)
        {
            var guid = Guid.NewGuid();
            outputPath += $"{Varietyof}{imageName}//{guid}.png";
            pictureData.OutPath = outputPath;
        }
        if (pictureData.OutPutType == OutputType.RandomVariety)
        {
            var guid = Guid.NewGuid();
            outputPath += $"{Varietyof}{imageName}//{guid}.png";
            pictureData.OutPath = outputPath;
        }
        if (pictureData.OutPutType == OutputType.MemeVariety)
        {
            var guid = Guid.NewGuid();
            outputPath += $"{Varietyof}{imageName}//{pictureData.OutPutType}{guid}{imageName}.png";
            pictureData.OutPath = outputPath;
        }
        if (pictureData.OutPutType == OutputType.Custom)
        {
            var guid = Guid.NewGuid();
            outputPath += $"{Varietyof}{imageName}//{guid}Custom of{imageName}";
            pictureData.OutPath = outputPath;
        }

        MagickImage outputImage = new();

        try
        {
            var fileBytes = await File.ReadAllBytesAsync(Path.GetFullPath(pictureData.FileName));
            outputImage = new MagickImage(fileBytes);
        }
        catch (Exception ex)
        {
            await settings.LogService.LogError($"Error in reading image into ImageMagick: {ex.Message}");
        }

        for (var box = 0; box < pictureData.BoxParameters.Count; box++)
        {
            if (pictureData.BoxParameters[box].CurrentBox.Type == BoxType.None)
            {
                break;
            }

            var boxParam = pictureData.BoxParameters[box];
            pictureData.MakeTextSettings(pictureData.BoxParameters[box]);

            await Task.Run(() =>
            {
                if (pictureData.OutPutType == OutputType.MemeVariety && pictureData.BoxParameters[box].Meme != null)
                {
                    MagickImage meme = new(boxParam.Meme);
                    meme.Resize(boxParam.CurrentBox.Width, boxParam.CurrentBox.Height);
                    outputImage.Composite(meme, boxParam.CurrentBox.X, boxParam.CurrentBox.Y, CompositeOperator.Over);
                }
                else if (pictureData.BoxParameters[box].Meme == null)
                {
                    //// Add the caption layer on top of the background image
                    var caption = new MagickImage($"caption:{boxParam.Text}", pictureData.ReadSettings);

                    var takeX = boxParam.CurrentBox.X;
                    var takeY = boxParam.CurrentBox.Y;

                    outputImage.Composite(caption, takeX, takeY, CompositeOperator.Over);
                }

                settings.LogService.LogInformation($"Picture:{Path.GetFileName(pictureData.FileName)}Box Type:{pictureData.BoxParameters[box].CurrentBox.Type} Box: {box + 1} of {pictureData.BoxParameters.Count} has been composited");
            });
        }

        // Annotate the image and handle annotation errors asynchronously
        await Task.Run(async () =>
        {
            try
            {
                outputImage.Annotate("Bulk Thumbnail Creator", gravity: Gravity.North);
            }
            catch (Exception ex)
            {
                // Handle annotation error asynchronously
                await settings.LogService.LogError($"Error in annotation: {ex.Message}");
            }
        });

        outputImage.Quality = 100;

        // outputs the file to the provided path and name
        await Task.Run(() => outputImage.Write(outputPath));
        await settings.LogService.LogInformation($"File Created: {outputPath}");
    }
}
