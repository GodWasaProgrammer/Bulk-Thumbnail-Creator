namespace BulkThumbnailCreator;

internal class FFmpegHandler
{
    public FFmpegHandler(ILogService logService)
    {
        _logService = logService;
    }

    ILogService _logService;

    /// <summary>
    /// This runs the process of ffmpeg and feeds it with parameters for file extraction
    /// </summary>
    /// <param name="parameters"></param>
    private async Task ExecuteFFMPG(string parameters, Settings settings)
    {
        Process processFFMpeg = new();

        processFFMpeg.StartInfo.FileName = settings.FfmpegDir;
        processFFMpeg.StartInfo.WorkingDirectory = settings.YTDLOutPutDir;
        processFFMpeg.StartInfo.Arguments = parameters;
        processFFMpeg.StartInfo.UseShellExecute = false;
        processFFMpeg.StartInfo.CreateNoWindow = false;
        processFFMpeg.StartInfo.RedirectStandardOutput = true;

        var processStarted = processFFMpeg.Start();

        if (!processStarted)
        {
            await _logService.LogError("FFMpeg failed to start");
            return;
        }
        else
        {
            await _logService.LogInformation("FFMpeg started");
        }

        await Task.Run(() => processFFMpeg.WaitForExit());

        await _logService.LogInformation("Ffmpeg finished producing pictures");
    }

    /// <summary>
    /// This runs FFMpeg.exe and extracts the files requested based on the CLI input
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="outPath"></param>
    public async Task RunFFMPG(Dictionary<string, string> parameters, string outPath, Settings settings)
    {
        var exePars = "";
        foreach (var parameter in parameters)
        {
            exePars += "-" + parameter.Key;
            exePars += " " + parameter.Value + " ";
        }

        exePars += outPath;

        await ExecuteFFMPG(exePars, settings);
    }
}
