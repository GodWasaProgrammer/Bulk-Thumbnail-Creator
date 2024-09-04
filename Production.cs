// Ignore Spelling: ytdl Exe

namespace BulkThumbnailCreator;

public static class Production
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

        if (File.Exists(YtdlpDir))
        {
            await settings.LogService.LogInformation($"YTDLP has been confirmed");
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
            await settings.LogService.LogInformation($"FFmpeg Has been confirmed");
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
            await settings.LogService.LogInformation($"YTDL has been confirmed");
        }
        else
        {
            await settings.LogService.LogError("YTDL Dir was not found");
            Directory.CreateDirectory(ytdlDir);
            await settings.LogService.LogInformation($"YTDL Dir has been created");
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
    public static async Task YouTubeDL(Job job)
    {
        var ytdl = new YoutubeDL
        {
            // set paths
            YoutubeDLPath = job.Settings.YTDLPDir,
            FFmpegPath = job.Settings.FfmpegDir,
            OutputFolder = job.Settings.YTDLOutPutDir,
        };

        // downloads specified video from youtube if it does not already exist.
        RunResult<string> res;

        ytdl.OverwriteFiles = false;

        if (job.VideoUrl == null)
        {
            await job.Settings.LogService.LogError("URL has been passed as null to YTDL");
            throw new ArgumentNullException(nameof(job.VideoUrl));
        }
        else
        {
            await job.Settings.LogService.LogInformation($"Attempting download of: {job.VideoUrl}");
            res = await ytdl.RunVideoDownload(url: job.VideoUrl);
        }

        await job.Settings.LogService.LogInformation("Download Success:" + res.Success.ToString());

        // sets BTC to run on the recently downloaded file res.data is the returned path.
        job.Settings.PathToVideo = res.Data;
    }

    public static void CreateDirectories(Settings settings)
    {
        if (!Directory.Exists(settings.OutputDir))
        {
            settings.LogService.LogInformation($"{settings.OutputDir} Directory was missing, will be created");
            Directory.CreateDirectory(settings.OutputDir);
        }
        if (!Directory.Exists(settings.TextAddedDir))
        {
            settings.LogService.LogInformation($"{settings.TextAddedDir} Directory was missing, will be created");
            Directory.CreateDirectory(settings.TextAddedDir);
        }
        if (!Directory.Exists(settings.YTDLOutPutDir))
        {
            settings.LogService.LogInformation($"{settings.YTDLOutPutDir} Directory was missing, will be created");
            Directory.CreateDirectory(settings.YTDLOutPutDir);
        }
    }

    /// <summary>
    /// Produces Picture using the ImageMagick Library
    /// </summary>
    /// <param name="pictureData">PictureDataObject Containing everything needed to create an image</param>
    public static async Task ProduceTextPictures(PictureData pictureData, Settings settings)
    {
        var outputPath = BuildFileName(pictureData, settings);
        var outputImage = await CreateImage(pictureData, settings);

        for (var box = 0; box < pictureData.BoxParameters.Count; box++)
        {
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
                else if (pictureData.BoxParameters[box].Meme == null && pictureData.BoxParameters[box].CurrentBox.Type != BoxType.None)
                {
                    //// Add the caption layer on top of the background image
                    var caption = new MagickImage($"caption:{boxParam.Text}", pictureData.ReadSettings);

                    // gradient section
                    if (boxParam.Gradient)
                    {
                        using var sparseColorImage = CreateGradient(boxParam, caption); // -sparse - color Barycentric

                        caption.Composite(sparseColorImage, CompositeOperator.In); // -compose In -composite
                    }
                    var takeY = boxParam.CurrentBox.Y;
                    var takeX = boxParam.CurrentBox.X;
                    if (boxParam.Shadows)
                    {
                        using var largerCanvas = CreateShadow(pictureData, boxParam, caption);
                        if (boxParam.Shadows)
                        {
                            outputImage.Composite(largerCanvas, takeX, takeY, CompositeOperator.Over);
                        }
                    }
                    if (!boxParam.Shadows)
                    {
                        outputImage.Composite(caption, takeX, takeY, CompositeOperator.Over);
                    }
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
        await settings.LogService.LogInformation($"File Created: {Path.GetFileName(outputPath)}");
    }

    private static MagickImage CreateShadow(PictureData pictureData, ParamForTextCreation boxParam, MagickImage caption)
    {
        // Create a larger canvas to accommodate the shadow
        var largerCanvas = new MagickImage(MagickColors.Transparent, caption.Width + 50, caption.Height + 50);
        // Composite the text onto the larger canvas
        largerCanvas.Composite(caption, 10, 10, CompositeOperator.Over);

        // Create a shadow image with larger width
        using (var shadow = new MagickImage(MagickColors.Transparent, largerCanvas.Width, largerCanvas.Height))
        {
            var shadowcaption = new MagickImage($"caption:{boxParam.Text}", pictureData.ReadSettings);

            // Clone the text image onto the shadow image
            shadow.Composite(caption, 0, 0, CompositeOperator.Over);

            // shadow 2
            shadow.Shadow(0, 0, 6, new Percentage(200), MagickColors.Black);
            largerCanvas.Composite(shadow, 10, 0, CompositeOperator.DstOver);

            // shadow 2
            shadow.Shadow(0, 0, 6, new Percentage(200), MagickColors.Black);
            largerCanvas.Composite(shadow, 10, 0, CompositeOperator.DstOver);

            // if you want the shadow to be offset you have to do it here
            // Composite the shadow behind the text on the larger canvas
            largerCanvas.Composite(shadow, 10, 0, CompositeOperator.DstOver);
        }

        return largerCanvas;
    }

    private static IMagickImage<ushort> CreateGradient(ParamForTextCreation boxParam, MagickImage caption)
    {
        var sparseColorImage = caption.Clone(); // +clone
        var fillColor = ColorData.MakeQuantumColor(boxParam.FillColor);
        var strokeColor = ColorData.MakeQuantumColor(boxParam.StrokeColor);
        var sparseColorArgs = new SparseColorArg[2];
        sparseColorArgs[0] = new SparseColorArg(0, caption.Height, fillColor);
        sparseColorArgs[1] = new SparseColorArg(caption.Width, 0, strokeColor);
        sparseColorImage.SparseColor(SparseColorMethod.Bilinear, sparseColorArgs);
        return sparseColorImage;
    }

    /// <summary>
    /// Creates our image object to read size
    /// </summary>
    /// <param name="pictureData"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public static async Task<MagickImage> CreateImage(PictureData pictureData, Settings settings)
    {
        MagickImage outputImage;

        try
        {
            var fileBytes = await File.ReadAllBytesAsync(Path.GetFullPath(pictureData.FileName));
            outputImage = new MagickImage(fileBytes);
        }
        catch (Exception ex)
        {
            outputImage = null;
            await settings.LogService.LogError($"Error in reading image into ImageMagick: {ex.Message}");
        }

        return outputImage;
    }
    /// <summary>
    /// Builds our filenames for ProduceTextPictures
    /// </summary>
    /// <param name="pictureData">Source to create the name from</param>
    /// <param name="settings">Settings to build the correct directory structure</param>
    /// <returns></returns>
    public static string BuildFileName(PictureData pictureData, Settings settings)
    {
        string imageName;
        var outputPath = Path.GetRelativePath(Environment.CurrentDirectory, settings.TextAddedDir);

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
        if (pictureData.OutPutType is OutputType.FontVariety)
        {
            var guid = Guid.NewGuid();
            outputPath += $"{Varietyof}{imageName}/FontVariety";
            Directory.CreateDirectory(outputPath);
            outputPath += $"/{guid}.png";
            pictureData.OutPath = outputPath;
        }
        if (pictureData.OutPutType is OutputType.ColorVariety)
        {
            var guid = Guid.NewGuid();
            outputPath += $"{Varietyof}{imageName}/ColorVariety";
            Directory.CreateDirectory(outputPath);
            outputPath += $"/{guid}.png";
            pictureData.OutPath = outputPath;
        }
        if (pictureData.OutPutType is OutputType.BoxVariety)
        {
            var guid = Guid.NewGuid();
            outputPath += $"{Varietyof}{imageName}/BoxVariety";
            Directory.CreateDirectory(outputPath);
            outputPath += $"/{guid}.png";
            pictureData.OutPath = outputPath;
        }
        if (pictureData.OutPutType is OutputType.FXVariety)
        {
            var guid = Guid.NewGuid();
            outputPath += $"{Varietyof}{imageName}/FXVariety";
            Directory.CreateDirectory(outputPath);
            outputPath += $"/{guid}.png";
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

        return outputPath;
    }
}
