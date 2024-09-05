namespace BulkThumbnailCreator;

public class Settings() : ISettings
{
    public bool Mocking { get; set; } = false;
    public bool MakeMocking { get; set; } = true;
    public string OutputDir { get; set; } = "output";
    public string TextAddedDir { get; set; } = "TextAdded";
    public string YTDLPDir { get; set; }
    public string YTDLOutPutDir { get; set; } = "YTDL";
    public string FfmpegDir { get; set; }
    public string[] Files { get; set; }
    public string DankMemeStashDir { get; set; } = "DankMemeStash";
    public string[] Memes { get; set; }
    public List<string> ListOfText { get; set; } = [];
    public string PathToVideo { get; set; }
}
