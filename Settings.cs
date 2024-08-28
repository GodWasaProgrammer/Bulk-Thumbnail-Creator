namespace BulkThumbnailCreator;

public class Settings(ILogService logger, JobService jobService) : ISettings
{
    public bool Mocking { get; set; } = false;
    public bool MakeMocking { get; set; } = true;
    public ILogService LogService { get; set; } = logger;
    public JobService JobService { get; set; } = jobService;
    public string OutputDir { get; set; } = "output";
    public string TextAddedDir { get; set; } = "TextAdded";
    public string YTDLPDir { get; set; }
    public string YTDLOutPutDir { get; set; } = "YTDL";
    public string FfmpegDir { get; set; }
    public string[] Files { get; set; }
    public string DankMemeStashDir { get; set; } = "DankMemeStash";
    public string[] Memes { get; set; }
    public List<PictureData> PictureDatas { get; set; } = [];
    public List<string> ListOfText { get; set; } = [];
    public string PathToVideo { get; set; }
    public List<string> DownloadedVideosList { get; set; } = [];
}
